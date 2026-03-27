SoloDB 1.1 shipped with typed document relations — `DBRef<T>`, `DBRefMany<T>`, delete policies, cascade insert, nested savepoints. The relation model worked. The query translation did not keep up.

Basic relation predicates translated fine. `.Where(o => o.Customer.Value.Name == "Alice")` became a JOIN. `.Any(t => t.Label == "News")` over a `DBRefMany` became an EXISTS subquery.

Then I tried to go further. Aggregation over collection references. Ordered subsets. Grouped joins. Nested relation predicates three hops deep. The kind of queries that LINQ lets you write because the type system permits them, and that a real database should be able to answer.

That was the point where the `StringBuilder`-based translator stopped being enough for what I needed next.

## The wall

In 1.1, the query translator assembled SQL using a `StringBuilder`. For simple shapes — a JOIN here, an `EXISTS` subquery there — that worked well enough. But once a query needed broader structure — ordered subsets, aggregates over `DBRefMany`, grouped joins, nested predicates — the problem was no longer writing one more SQL fragment. The problem was that the query only existed as concatenated text, with nothing structured to validate, simplify, or rewrite before it reached SQLite.

I could have kept extending it. Add another special case for `GroupJoin`. Another branch for nested `Any`. Another code path for each new shape. Every extension would produce SQL that might be correct, and I would find out when a user reported wrong results.

That was the real problem. Not that certain queries threw exceptions — that is manageable. The problem was the queries that would run, return plausible-looking results, and be wrong. For a database, that is the worst kind of failure.

I decided to rebuild the translator.

## The decision

Once that was clear, I stopped extending the old translator and rebuilt it around a typed representation. The query had to exist as structure before it existed as SQL text.

I built it in F#, using .NET discriminated unions. That made the SQL shapes explicit, made the engine easier to reason about, and much harder to add new forms without updating every part of the pipeline that needs to know about them.

The result is what I call the SQL DU Engine. The contract layer uses discriminated unions such as `SqlExpr`, `JoinShape`, and `TableSource`, plus the `SelectCore` record, to describe every SQL shape SoloDB knows how to emit. If a query cannot be represented by these types, it cannot reach SQLite. That is how unsupported shapes get caught.

Once the query exists as a typed structure, the engine can validate it, simplify it, and rewrite it before emitting anything. That is what the earlier `StringBuilder`-based SQL generator was not built to do. The optimizer runs multiple passes — folding constants, flattening subqueries, pushing predicates through boundaries, narrowing projections — in a loop until no pass produces a simpler result. Then the emitter walks the typed tree and writes the final SQLite SQL.

## What it enabled

With the typed path in place, the supported `DBRefMany` LINQ surface expanded substantially. Filtering, aggregation, ordering, paging, projection, set operations, and grouped joins over `DBRefMany` now translate to SQL for supported compositions. Not because I added more special cases to the old generator, but because the typed representation can express these shapes and the optimizer can simplify them before emission.

Here is a concrete example. Find all ledgers where the three smallest expenses sum to exactly 60:

```csharp
var result = ledgers
    .Where(l => l.Expenses
        .OrderBy(e => e.Amount)
        .Take(3)
        .Sum(e => e.Amount) == 60m)
    .ToList();
```

That translates to a single SQLite statement. The `OrderBy`, `Take`, and `Sum` over the `DBRefMany` collection all stay inside the database.

`GroupJoin` works the same way. For each event, count tickets and sum VIP revenue:

```csharp
var summary = events.AsQueryable()
    .GroupJoin(
        tickets.AsQueryable(),
        e => e.Id,
        t => t.EventId,
        (e, group) => new
        {
            e.Title,
            TicketCount = group.Count(),
            VipRevenue = group.Where(t => t.Tier == "vip").Sum(t => t.Price)
        })
    .OrderBy(x => x.Title)
    .ToList();
```

Nested relation queries — `DBRefMany` through `DBRef` through `DBRefMany` — translate to correlated subqueries in one SQLite statement. No N+1.

And when a shape is outside the supported surface, SoloDB throws `NotSupportedException` during translation, before the query runs. No silent fallback to client-side evaluation. No partial results that look correct and are not.

## Where it landed

The work ended at 4605 tests with zero failures. More queries stay inside SQLite. The boundaries are explicit.

That is what moving from a string builder to a typed representation actually changes. Not the feature count — the confidence that the features are correct.
