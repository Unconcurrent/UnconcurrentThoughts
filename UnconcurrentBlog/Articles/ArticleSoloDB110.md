You picked a document database for the flexibility. No migrations, no rigid schemas, just objects in and objects out. It works beautifully right up to the moment you need one collection to reference another.

Then you discover the gap that document databases keep handing back to application code: referential integrity. You check whether targets still exist before you use them. You write cleanup jobs for orphans and null guards for records deleted three days ago. None of that logic is transactional, none of it is enforced by the engine, and all of it becomes your problem forever.

SoloDB 1.1 moves that burden into the database engine. References are typed. Deletes are policy-governed. Queries cross collection boundaries in LINQ. Nested work uses SQLite savepoints instead of retry-everything application code.

## Typed Relations

The first thing that breaks in a document database without relation support is the shape of your model. You start storing foreign keys as raw longs or strings, writing manual lookups every time you need the referenced object, and hoping the types survive the next refactor.

Most document databases leave this to you because they store documents as isolated blobs. Even when a reference helper exists, it usually behaves like a convenience wrapper around an id field, not like a real engine-level relation.

SoloDB models references as part of the type system. A single reference is `DBRef<T>`. A collection reference is `DBRefMany<T>`. The engine persists them through dedicated link tables and can query through them because the relation exists below your service layer, not only in your C# conventions.

```csharp
using SoloDatabase;
using SoloDatabase.Attributes;

// Define our schema.
public sealed record Customer(long Id, string Name);
public sealed record OrderItem(long Id, string Sku, int Quantity);

public sealed class Order
{
    public long Id { get; set; }
    public string Number { get; set; } = string.Empty;
    // OnOwnerDelete applies to both DBRef and DBRefMany.
    [SoloRef(OnOwnerDelete = DeletePolicy.Unlink)]
    public DBRef<Customer> Customer { get; set; }
        = DBRef<Customer>.None;
    public DBRefMany<OrderItem> Items { get; set; } = new();
}

using var db = new SoloDB("memory:blog-110");
var orders = db.GetCollection<Order>();
var customers = db.GetCollection<Customer>();

// To references an existing entity; From creates one inline.
var adaId = customers.Insert(new Customer(Id: 0, "Ada")); // Id 0 = auto-assign

var order = new Order { Number = "ORD-1001" };
order.Customer = DBRef<Customer>.To(adaId);
order.Items.Add(new OrderItem(0, "BOOK-1", 2));
order.Items.Add(new OrderItem(0, "PEN-9", 5));
orders.Insert(order);

var matches = orders
    .Where(o =>
        o.Customer.Value.Name == "Ada"
        && o.Items.Any(i => i.Quantity >= 2))
    .ToList();

Console.WriteLine($"{matches[0].Number}: matched");
// ORD-1001: matched
```

`DBRef.To(...)` points to an existing entity. `DBRef.From(...)` creates one inline. Either way, inserting the order wires the references and cascades any new items in one transaction.

That gets data in cleanly. The harder problem is what happens when you delete something.

## Delete Safety

Every developer who has stayed with a document database long enough has written some version of this: before deleting a record, search every collection that might reference it, then either block the delete or try to clean up the orphaned data. It is tedious, easy to miss on one path, and exactly the sort of thing that rots in service code.

The structural reason is simple. If the storage layer has no relation metadata, the engine cannot enforce referential rules for you. Cleanup lives in application code because the database does not know what should happen.

SoloDB persists relations through dedicated link tables and relation metadata. That gives the engine enough information to enforce delete policies the way a relational database enforces foreign key rules, without giving up document-shaped models. The default target-side rule is `Restrict`. The default owner-side rule is `Deletion`. If your domain needs something else, define it once with `[SoloRef(...)]` and the engine carries it from there.

```csharp
using SoloDatabase;
using SoloDatabase.Attributes;

public sealed record Member(long Id, string Name);

public sealed class Team
{
    public long Id { get; set; }
    // These are the defaults, shown for reference.
    [SoloRef(OnDelete = DeletePolicy.Restrict,
        OnOwnerDelete = DeletePolicy.Deletion)]
    public DBRef<Member> Lead { get; set; } = DBRef<Member>.None;
}

var teams = db.GetCollection<Team>();
var members = db.GetCollection<Member>();

var teamId = teams.Insert(new Team {
    Lead = DBRef<Member>.From(new Member(0, "Ada"))
});

var memberId = teams.GetById(teamId).Lead.Id;

try { members.Delete(memberId); }
catch (InvalidOperationException) { } // blocked by Restrict

teams.Delete(teamId); // owner delete triggers Deletion
```

That is the practical gap with LiteDB's `DbRef`: references exist, but SoloDB adds database-enforced delete behavior instead of leaving referential cleanup to application code.

When the rule lives in the engine, you stop rediscovering the same delete bug in every service boundary.

## SoloRef Policy Reference

`[SoloRef]` controls what the engine does when a referenced entity or its owner is deleted.

**OnDelete** — when the *referenced* (target) entity is deleted:

| Value | Effect |
|---|---|
| `Restrict` (default) | Block the delete if any references point to it. |
| `Cascade` | Also delete every entity that holds a reference to the target. |
| `Unlink` | Remove the reference (set `DBRef` to `None`, remove link rows for `DBRefMany`). Referenced entities survive. |

**OnOwnerDelete** — when the *owning* entity is deleted:

| Value | Effect |
|---|---|
| `Deletion` (default) | Unlink first, then delete each formerly-linked target whose global reference count across all collections drops to zero. Targets still referenced elsewhere survive. |
| `Unlink` | Remove link rows only. All referenced entities survive unconditionally. |
| `Restrict` | Block owner deletion if any links exist. |

`Cascade` is not valid for `OnOwnerDelete` — use `Deletion` instead.

## Nested Rollback

Once relations matter, workflows become multi-step by default. You insert a parent, derive related records, enrich metadata, and then something in the middle fails. In a flat transaction model the choices are ugly: roll everything back or start writing compensating code.

Many document databases stop at a single transaction boundary. They can commit or abort, but they do not give you a structured way to let one inner step fail without throwing away the outer work that should survive.

SoloDB uses SQLite savepoints for nested `WithTransaction(...)` calls. An inner scope becomes a savepoint, not a second transaction. If the inner work throws, only that inner scope is rolled back. The outer transaction can catch the failure, continue, and still commit the valid work around it.

```csharp
using SoloDatabase;

// AuditEntry has Id/Message.
db.WithTransaction(tx =>
{
    var audit = tx.GetCollection<AuditEntry>();
    audit.Insert(new AuditEntry { Message = "outer-start" });

    try
    {
        tx.WithTransaction(inner => {
            inner.GetCollection<AuditEntry>()
                .Insert(new AuditEntry { Message = "inner-temp" });
            throw new InvalidOperationException();
        });
    }
    catch (InvalidOperationException) { }

    audit.Insert(new AuditEntry { Message = "outer-commit" });
});

// Persisted rows: outer-start, outer-commit
```

The inner insert vanishes. The outer work survives. That is the difference between retrying entire workflows and handling failure where it actually happened.

## Querying Across Relations

Typed references, delete policies, and nested rollback all matter less if every cross-collection query still collapses into loading both sides and filtering in memory. That is exactly what most document databases force you to do: store a reference, load the owner, load the target, stitch them together in application code, and hope the dataset stays small enough for the approach to hold.

It turns what should be a database capability into service-layer debt. Every relation-aware query becomes another place where correctness and performance depend on handwritten loading logic.

SoloDB translates supported relation predicates into SQL joins through the same link-table metadata it uses for insert and delete. The query in the first example, `o.Customer.Value.Name == "Ada" && o.Items.Any(...)`, is not a cosmetic convenience. SQLite evaluates it as part of the real query:

```sql
SELECT "Order".Id,
  -- Hydrate the Customer reference inline: replace the stored foreign key
  -- with the full joined object so the deserialized entity is ready to use.
  jsonb_set("Order".Value, '$.Customer',
    CASE WHEN _ref0.Id IS NOT NULL
      THEN jsonb_array(_ref0.Id, _ref0.Value)
      ELSE jsonb_extract("Order".Value, '$.Customer')
    END) AS Value
FROM "Order"
LEFT JOIN "Customer" AS _ref0
  ON _ref0.Id = jsonb_extract("Order".Value, '$.Customer')
WHERE jsonb_extract(_ref0.Value, '$.Name') = @name
  AND EXISTS (
    SELECT 1
    FROM "SoloDBRelLink_Order_Items" AS _lnk
    INNER JOIN "OrderItem" AS _tgt
      ON _tgt.Id = _lnk.TargetId
    WHERE _lnk.SourceId = "Order".Id
      AND jsonb_extract(_tgt.Value, '$.Quantity') >= @qty
  )
```

The single-ref `Customer` becomes a LEFT JOIN. The many-ref `Items.Any(...)` becomes a correlated EXISTS over the link table. Both paths use the same relation metadata the engine already maintains for insert and delete.

No other embedded .NET document database offers this full combination today: typed references, enforced delete policies, nested savepoints, and LINQ-queryable relation predicates in one embedded library.

That is the point of SoloDB 1.1. You keep the flexibility that made you choose a document model in the first place without inheriting relation correctness as application debt.

## Getting Started

Install from NuGet:

```bash
dotnet add package SoloDB
```

Then copy the first example into a console project, replace `"memory:blog-110"` with a file path, and you have a working relational document database. The [full relations documentation](https://solodb.org/docs.html#relations) covers the broader API surface, including `Include` and `Exclude`, `UpdateMany` relation diffs, and the full delete policy matrix.
