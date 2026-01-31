## Preface

SoloDB has reached version 1.0. Two years of production use. A 1.5TB database. Zero critical failures. It's stable.

This article covers what 1.0 includes, the new Events API, and what the version number means for developers building on SoloDB.

## The Journey to Stability

SoloDB was created to address a specific problem: combining NoSQL flexibility with SQL reliability. Building directly upon [SQLite's JSONB](https://sqlite.org/jsonb.html) format allowed the project to inherit decades of battle-tested optimization while providing a modern document database API.

The 1.0 delay was deliberate. Passing tests isn't enough — sustained production use exposes edge cases no test suite anticipates. Two years of that have passed.

Key milestones:
1. **Initial release** — Core document storage with LINQ support
2. **File storage** — Hierarchical virtual filesystem with Snappy compression
3. **Polymorphic collections** — Store and query derived types in single collections
4. **Events API** — Reactive patterns via SQLite triggers
5. **1.0** — Public API stable, production-ready

## The Complete Feature Set

### LINQ Support

SoloDB provides standard `IQueryable<T>` support that translates directly to SQL:

```csharp
var users = db.GetCollection<User>();

// Complex queries translate to optimized SQL
var activeAdmins = users
    .Where(u => u.IsActive && u.Role == "Admin")
    .OrderByDescending(u => u.LastLogin)
    .Take(10)
    .ToList();

// Projections work as expected
var emails = users
    .Where(u => u.Department == "Engineering")
    .Select(u => u.Email)
    .ToList();
```

The generated SQL uses indexes when available. Use `SoloDB.GetSQL(...)` to inspect the translation and `SoloDB.ExplainQueryPlan(...)` to verify index usage.

### Transactions

SQLite's proven transaction system is exposed through a straightforward API:

```csharp
db.WithTransaction(tx => {
    var accounts = tx.GetCollection<Account>();

    var source = accounts.GetById(sourceId);
    var target = accounts.GetById(targetId);

    source.Balance -= amount;
    target.Balance += amount;

    accounts.Update(source);
    accounts.Update(target);

    // Commits on success, rolls back on exception
});
```

### File Storage

A hierarchical virtual filesystem with transactional guarantees:

```csharp
var fs = db.FileSystem;

// Upload with automatic compression
using var stream = File.OpenRead("report.pdf");
fs.Upload("/reports/2024/quarterly.pdf", stream);

// Metadata support
fs.SetMetadata("/reports/2024/quarterly.pdf", "Author", "Finance Team");

// Recursive listing
var allReports = fs.RecursiveListEntriesAt("/reports/").ToList();

// Hash-based lookup
var fileByHash = fs.GetFileByHash(knownHash);
```

Files are chunked, compressed with [Snappy](https://github.com/brantburnett/Snappier), and SHA-1 hashed for integrity verification.

### Polymorphic Collections

Store objects of different derived types within a single collection:

```csharp
public abstract class Shape { public long Id { get; set; } }
public class Circle : Shape { public double Radius { get; set; } }
public class Rectangle : Shape { public double Width { get; set; } public double Height { get; set; } }

var shapes = db.GetCollection<Shape>();
shapes.Insert(new Circle { Radius = 5.0 });
shapes.Insert(new Rectangle { Width = 4.0, Height = 6.0 });

// Query by derived type
var circles = shapes.OfType<Circle>().Where(c => c.Radius > 3.0).ToList();
```

### Indexing

Attribute-based or programmatic index creation:

```csharp
public class User
{
    public long Id { get; set; }

    [Indexed(unique: true)]
    public string Email { get; set; }

    [Indexed]
    public string Department { get; set; }
}

// Or create indexes at runtime
users.EnsureIndex(u => u.LastLogin);
users.EnsureUniqueIndex(u => u.Username);
```

### Custom ID Generation

Extensible ID strategy support:

```csharp
public class GuidIdGenerator : IIdGenerator<Document>
{
    public object GenerateId(ISoloDBCollection<Document> collection, Document item)
        => Guid.NewGuid().ToString("N");

    public bool IsEmpty(object id) => string.IsNullOrEmpty(id as string);
}

public class Document
{
    [SoloId(typeof(GuidIdGenerator))]
    public string Id { get; set; }

    public string Content { get; set; }
}
```

## The Events API

New to this release is the Events API — reactive patterns within your data layer, implemented via SQLite triggers.

### The Six Lifecycle Hooks

| Event | Timing | Use Case |
|-------|--------|----------|
| `OnInserting` | Before insert | Validation, enrichment |
| `OnInserted` | After insert | Notifications, logging |
| `OnUpdating` | Before update | Audit trails, validation |
| `OnUpdated` | After update | Cache invalidation |
| `OnDeleting` | Before delete | Cascade checks, archival |
| `OnDeleted` | After delete | Cleanup, notifications |

### Validation with Rollback

```csharp
var users = db.GetCollection<User>();

users.OnInserting(ctx => {
    if (string.IsNullOrEmpty(ctx.Item.Email))
        throw new InvalidOperationException("Email is required");

    if (!ctx.Item.Email.Contains("@"))
        throw new InvalidOperationException("Invalid email format");

    return SoloDBEventsResult.EventHandled;
});
```

Throwing an exception rolls back the entire operation. The handler remains registered for future operations.

### Cross-Collection Operations

The event context implements `ISoloDB`, enabling transactional operations across collections:

```csharp
users.OnUpdating(ctx => {
    var auditLog = ctx.GetCollection<AuditEntry>();

    auditLog.Insert(new AuditEntry {
        Timestamp = DateTimeOffset.UtcNow,
        EntityType = "User",
        EntityId = ctx.Item.Id,
        OldValue = ctx.OldItem.Name,
        NewValue = ctx.Item.Name
    });

    return SoloDBEventsResult.EventHandled;
});
```

### One-Shot Handlers

Return `RemoveHandler` to auto-unregister after execution:

```csharp
users.OnInserted(ctx => {
    Console.WriteLine($"First user created: {ctx.Item.Id}");
    return SoloDBEventsResult.RemoveHandler; // Runs once, then unregisters
});
```

### Handler Behavior

- **Exception handling**: When a handler throws, execution stops and the operation rolls back. Remaining handlers are skipped. The throwing handler remains registered.
- **Multiple handlers**: Execute in registration order until one throws or all complete.
- **Transaction scope**: All events (including "after" events) run within the same SQLite transaction.

## The Architecture

SoloDB builds directly on SQLite rather than implementing a custom storage engine. This provides:

1. **Battle-tested reliability** — SQLite is the most deployed database in existence, used in browsers, mobile devices, and spacecraft.
2. **Proven transactions** — Full ACID compliance with decades of edge-case handling.
3. **Efficient storage** — JSONB provides compact binary JSON with fast extraction.
4. **Tooling compatibility** — Any SQLite tool can inspect your database.
5. **Automatic improvements** — Every SQLite optimization benefits SoloDB transitively.

The minimum required SQLite version is 3.47.0, which provides JSONB support and trigger RAISE() expressions.

## The Stability Commitment

The public API will remain stable to a reasonable degree — bug fixes ship, internal-but-public details may evolve, but your code won't break on minor updates.

## Getting Started

Install from NuGet:

```bash
dotnet add package SoloDB
```

Basic usage:

```csharp
using SoloDatabase;

// Create or open a database
using var db = new SoloDB("myapp.db");

// Get a typed collection
var users = db.GetCollection<User>();

// Insert
var user = new User { Name = "Alice", Email = "alice@example.com" };
users.Insert(user);
Console.WriteLine($"Created user with ID: {user.Id}");

// Query
var found = users.FirstOrDefault(u => u.Email == "alice@example.com");

// Update
found.Name = "Alice Smith";
users.Update(found);

// Delete
users.Delete(found.Id);
```

Resources:
- [Documentation](https://solodb.org/docs.html) — Complete API reference
- [GitHub](https://github.com/Unconcurrent/SoloDB) — Source code, issues, discussions
- [NuGet](https://www.nuget.org/packages/SoloDB) — Package downloads
- [Comparison with LiteDB](https://unconcurrent.com/articles/SoloDBvsLiteDB.html) — Benchmarks and API differences

## The Verdict

SoloDB 1.0 isn't a promise — it's a fact. Two years of production, a terabyte of data, zero critical failures.

If you want NoSQL flexibility with SQLite reliability, and an API that feels like C# instead of fighting it — this is it.
