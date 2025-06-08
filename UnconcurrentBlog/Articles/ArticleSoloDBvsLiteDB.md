## Preface

After months of work, I finally present to you a new embedded database: SoloDB - an ACID, No-SQL and SQL hybrid on top of [SQLite's JSONB](https://sqlite.org/jsonb.html) format. It is a contender to any .NET document database, including LiteDB, MongoDB, and RavenDB, but on a smaller scale.

Let me share my experience and show you why SoloDB might be the better choice for your next project.

## Why was it created

SoloDB was created to simultaneously address these problems: NoSQL flexibility, SQL compatibility, full filesystem support, and strong ACID guarantees.
No existing .NET database offered all these features.

## The Comparison

I ran comprehensive benchmarks comparing SoloDB against LiteDB, while trying to make the tests as optimized as possible for both contenders. The results are appended to this document, and are positive for SoloDB.

### The Improvements That Matter

In a "GroupBy and count users by username's first letter" query, SoloDB is **57% faster** than LiteDB (21,50 ms vs 50,08 ms). But it's not just about raw speed - it's about ease of achieving these speeds.

LiteDB's [ILiteQueryable](https://github.com/litedb-org/LiteDB/blob/master/LiteDB/Client/Database/ILiteQueryable.cs#L8) interface limits the methods you can use and when, sometimes requiring a fallback to its [custom query language](https://www.litedb.org/api/).

SoloDB supports [39 different](https://github.com/Unconcurrent/SoloDB/blob/035ae3f7f0cb6774bc80e4498548efad287d5632/SoloDB/Queryable.fs#L17) nested [IQueryable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.linq.iqueryable-1) methods, all of which convert directly into SQL.

And the most dramatic improvement was memory allocation. For the same query, LiteDB allocates 30,37 MB while SoloDB uses just 56,05 KB - that's a **99.8% reduction**!

By leveraging SQLite's optimized query engine instead of reinventing the wheel, SoloDB achieves managed memory efficiency that LiteDB simply cannot match without a rewrite of the full project.

## The GroupBy Problem

SoloDB outperforms LiteDB, and it does so with style:

In SoloDB, the test is implemented with LINQ:

```csharp
// Natural, idiomatic LINQ
var letterCounts = users
    .GroupBy(x => x.Username[0])
    .Select(x => new { Key = x.Key, Count = x.Count() })
    .ToDictionary(k => k.Key.ToString(), e => e.Count);
```

This is clean, type-safe, known by all .NET engineers, and enforced at compile time.

But in LiteDB you need this *contraption*.

```csharp
// LiteDB's workaround using string-based expressions
var letterCounts = users
    .Query()
    .GroupBy(BsonExpression.Create("SUBSTRING(Username, 0, 1)"))
    .Select(BsonExpression.Create("{key: @key, count: COUNT(*)}"))
    .ToEnumerable()
    .ToDictionary(k => k["key"].AsString, e => e["count"].AsInt64);
```

I spent 10 minutes trying to get this to run within the LiteDB's engine, just to keep the benchmark fair. I couldn't figure out how to [select](https://www.litedb.org/api/query/) multiple fields, so I guessed until the JSON format worked. The syntax is opaque to a .NET developer.

This is disconnected from the language's type system, difficult to debug, and easy to get wrong: Imagine you've optimized your app using string-based queries. Later, after a refactor, these queries silently break. You don't know until it's too late. Now you must track them down, fix them by hand, and pray your tests catch everything. It's a mess.

## Real IQueryable Support

SoloDB, by contrast, provides standard `IQueryable` support that translates directly to SQL. No hacks, no special syntax — just pure LINQ. This means:

- Full IntelliSense integration
- No need to learn a special query language
- Compile-time type safety
- Predictable performance characteristics
- No need for *.AsEnumerable()* workarounds.

A query like `users.Where(x => x.Username.StartsWith("a"))` compiles to SQL, taking advantage of indexes when available. If needed, you can manually fine-tune for performance.

Here's the SQL SoloDB generates under the hood for that query, it was retrieved by using `SoloDB.GetSQL(...)`:

```sql
SELECT Id, Value as ValueJSON
FROM (
    SELECT Id, jsonb_extract("UserSoloDB".Value, '$') as Value
    FROM "UserSoloDB"
    WHERE (
        -- An index compatible search.
        jsonb_extract("UserSoloDB".Value, '$.Username') >= @V171
        AND jsonb_extract("UserSoloDB".Value, '$.Username') < @V231
    )
)
```

And its query plan with `SoloDB.ExplainQueryPlan(...)`:

```sql
SEARCH UserSoloDB USING INDEX UserSoloDB_index_jsonb_extractValueUsername (<expr>>? AND <expr><?)
```

## The Architecture Advantage

SoloDB's foundation is fundamentally different and better. By building on SQLite's JSONB support, we get:

1. **Battle-tested reliability**: Used in everything from browsers to spacecraft, SQLite is the most battle-tested database in existence.
2. **Proper transactions**: Full ACID compliance with SQLite's proven transaction system
3. **Efficient storage**: JSONB provides compact storage with fast access
4. **Compressing Filesystem**: The virtual filesystem is transparently compressed using [Snappy](https://github.com/brantburnett/Snappier).
5. **SQL compatibility**: You can use raw SQL when needed.
6. **Better tooling**: Any SQLite tool can inspect your database.

I claim no credit for these features — SoloDB simply stands on the shoulders of the giants behind SQLite.

## File System Integration That Makes Sense

Both databases support file storage, but SoloDB's integrated filesystem is far more elegant and powerful.

### With SoloDB's virtual filesystem, you can:

1. Retrieve entities by Unix-style paths.
2. List files and directories using a built-in index.
3. Recursively enumerate contents of any folder.
4. Write, move, and replace files & directories transactionally.
5. Open or copy files by path, as streams or simple read calls.
6. Attach and query metadata for richer file context.
7. Perform indexed hash-based file lookups.

### Retrieving 100 files with associated usernames and metadata

- LiteDB: 17,26 ms, 17,07 MB allocated;
- SoloDB:  2,46 ms,  6,99 KB allocated.

That's an 85,7% speed improvement and nearly 100% less memory.
Why? SoloDB's filesystem is not bolted on — it's baked into the SQL engine itself.

## Performance Across the Board

Let me share more benchmark highlights:

### Operations Where SoloDB Excels

- **Inserting 10,000 users**:
  - SoloDB is 29,3% faster, with 94,9% less memory allocation
- **Pagination queries**:
  - SoloDB is 36,7% faster, with 90,6% less generated memory
- **Delete operations**:
  - 26,1% faster and 91,9% less memory usage
- **Reading random file chunks**:
  - 41,0% faster, and 68,4% less memory used
- **Retrieving files with usernames and metadata**:
  - 85,7% faster, and essentially 100% less memory
- **Complex queries**:
  - 95,7% faster, and 99,9% less memory

### Cases Where LiteDB Has the Edge

- **Searching within array properties**
  LiteDB supports indexes on arrays; SoloDB does not — yet.

- **File write operations**
  LiteDB is faster when writing files due to its minimal overhead. SoloDB performs more integrity and metadata checks by design.

- **Very simple queries**
  For ultra-fast, cache-resident queries, LiteDB may outperform SoloDB due to smaller query to engine pipeline.

I admit that writing to the filestorage is faster on LiteDB, because SoloDB performs more work during file writes, including:

- Storing creation and modification timestamps.
- Updating all parent directory timestamps.
- Compressing file chunks with [Snappy](https://github.com/brantburnett/Snappier).
- Hashing contents for integrity and fast lookup.
- Enforcing valid path rules and length limits.
- Preventing illegal names (e.g., `..`, or malformed paths).

This tradeoff was intentional — I built SoloDB to prioritize integrity and completeness, even if it means sacrificing performance in a few places.

## The Developer Experience

Beyond performance, SoloDB offers a superior developer experience:

```csharp
// SoloDB - intuitive and discoverable
using var db = new SoloDB("./mydb.db");
var users = db.GetCollection<User>();

// Add an index on the count of uploaded files.
users.EnsureIndex(u => u.UploadedFiles.Count);

// Complex queries just work
var gamingFiles = users
    .Where(u => u.InterestedCategories.Contains("Gaming") 
                && u.UploadedFiles.Count > 5 /* Using the index */)
    // Complex projection methods like SelectMany are supported — they translate cleanly into SQL when used with simple selectors.
    .SelectMany(u => u.UploadedFiles) 
    .OrderBy(f => f)
    .Skip(1)
    .Take(10)
    .ToList();

// Transactions are simple: commit on return, fail on exception.
db.WithTransaction(tx => {
    var users = tx.GetCollection<User>();
    // Your transactional operations
});
```

No `BsonExpression`s, no custom syntax, no surprises — just C# that reads like C#.

## Advanced Features

Here are some modern features that were not highlighted yet.

1. **Custom ID generators**: Implement your own ID strategy for `Int64`, `String`, or `Guid` — such as Version 7 GUIDs.
2. **Attribute-based indexing**: Use `[Indexed]` attributes for simplicity, or create indexes manually via code.
3. **Hash based file lookup**: All files are SHA-1 content-hashed.
4. **Auto optimization**: On startup, SoloDB runs ```PRAGMA optimize;``` enabling SQLite to gather fresh statistics and optimize future queries.
5. **Object inheritance support**: Collections support base and derived types. An `Animal` collection can store a `Cat`, and type checks like ```animal.GetType() == typeof(Cat)``` are fully supported in queries.
6. **[Dapper](https://github.com/DapperLib/Dapper)-like interoperability methods**: Execute, Query\<T\>, QueryFirst\<T\>, ...

## The Verdict

After weighing the benchmarks, architecture, and day-to-day coding experience, one thing is clear: SoloDB is better.

Faster common operations, 90–99% lower memory usage overall, and seamless LINQ support that just feels right.
No fragile strings. No surprises during refactors. Just clean, expressive queries backed by the full power of SQLite.

That alone would be a strong case. But SoloDB also brings peace of mind: ACID transactions you can trust, a mature storage engine proven in thousands of applications, and first-class compatibility with the entire SQLite ecosystem.

Try it! Download the [SoloDB Nuget package](https://www.nuget.org/packages/SoloDB) and start building with it.

And if you don't like it?
Just delete it — and keep using your data directly with SQL.
After all, all the way down, it's just plain old SQLite.

## Data

- LiteDB(5.0.21) vs SoloDB(0.2.2)
- Tested on Win 11 on SSD with CPU AMD Ryzen 9
- .NET 9
- Test iterations: 3
- User count per test: 10_000
- The benchmark project is hosted on [GitHub](https://github.com/Unconcurrent/SoloDBvsLiteDB)

## COMPARISON OF RESULTS

### Category:  1.General

+---------+
| Step Name                                    | LiteDB Time | SoloDB Time |     Difference | LiteDB GC Alloc | SoloDB GC Alloc |    Difference |
|----------------------------------------------|-------------|-------------|----------------|-----------------|-----------------|---------------|
| Complex query with multiple conditions       |    24,84 ms |     1,08 ms |  SoloDB +95,7% |        17,94 MB |        24,01 KB | SoloDB -99,9% |
| Count users by username first letter         |    50,08 ms |    21,50 ms |  SoloDB +57,1% |        30,37 MB |        56,05 KB | SoloDB -99,8% |
| Delete users with <= 2 categories            |   255,12 ms |   188,62 ms |  SoloDB +26,1% |       133,10 MB |        10,76 MB | SoloDB -91,9% |
| Inserting 10000 users                        |   622,98 ms |   440,52 ms |  SoloDB +29,3% |       714,04 MB |        36,56 MB | SoloDB -94,9% |
| Optimize                                     |         N/A |     6,41 ms |  SoloDB  +Inf% |             N/A |        488,00 B | SoloDB  +Inf% |
| Paginated query (page 3 of 50 users)         |   716,60 μs |   453,30 μs |  SoloDB +36,7% |       175,23 KB |        16,40 KB | SoloDB -90,6% |
| Searching for gaming users.                  |    11,17 ms |    22,40 ms | LiteDB +100,5% |         2,45 MB |       153,84 KB | SoloDB -93,9% |
| Update users with username starting with 'a' |    17,41 ms |     7,10 ms |  SoloDB +59,2% |        10,49 MB |         9,69 KB | SoloDB -99,9% |
-------------------------------------------------------------------------------------------------------------------------------------------------

### Category:  2.FS

+---------+
| Step Name                                             | LiteDB Time | SoloDB Time |     Difference | LiteDB GC Alloc | SoloDB GC Alloc |     Difference |
|-------------------------------------------------------|-------------|-------------|----------------|-----------------|-----------------|----------------|
| Read 256 bytes chunk from random file positions       |    14,35 ms |     8,46 ms |  SoloDB +41,0% |         6,97 MB |         2,20 MB |  SoloDB -68,4% |
| Read first 1KB from all gaming files                  |   106,18 ms |    34,50 ms |  SoloDB +67,5% |        66,26 MB |        17,37 MB |  SoloDB -73,8% |
| Retrieve file&tags from users                         |    17,26 ms |     2,46 ms |  SoloDB +85,7% |        17,07 MB |         6,99 KB | SoloDB -100,0% |
| Upload 3 64 kb files for 200 gamers in a transaction. |   353,03 ms |      1,33 s | LiteDB +277,1% |       480,30 MB |       231,23 MB |  SoloDB -51,9% |
-----------------------------------------------------------------------------------------------------------------------------------------------------------


## LiteDB DETAILED BENCHMARK RESULTS

### Category:  1.General

+---------+
| Step Name                                    | N |  Time Avg |  Time Min |  Time Max | % Time | GC Alloc Avg | GC Alloc Min | GC Alloc Max |
|----------------------------------------------|---|-----------|-----------|-----------|--------|--------------|--------------|--------------|
| Inserting 10000 users                        | 3 | 622,98 ms | 307,95 ms |    1,22 s |   63,4 |    714,04 MB |    684,34 MB |    728,98 MB |
| Searching for gaming users.                  | 3 |  11,17 ms |   2,86 ms |  24,92 ms |    1,1 |      2,45 MB |      2,38 MB |      2,53 MB |
| Update users with username starting with 'a' | 3 |  17,41 ms |  12,89 ms |  23,66 ms |    1,8 |     10,49 MB |      9,88 MB |     11,45 MB |
| Delete users with <= 2 categories            | 3 | 255,12 ms | 187,10 ms | 295,27 ms |   26,0 |    133,10 MB |    131,21 MB |    134,43 MB |
| Paginated query (page 3 of 50 users)         | 3 | 716,60 μs | 330,00 μs |   1,20 ms |    0,1 |    175,23 KB |    166,11 KB |    191,61 KB |
| Complex query with multiple conditions       | 3 |  24,84 ms |  17,10 ms |  39,46 ms |    2,5 |     17,94 MB |     17,89 MB |     18,03 MB |
| Count users by username first letter         | 3 |  50,08 ms |  34,90 ms |  76,63 ms |    5,1 |     30,37 MB |     30,30 MB |     30,49 MB |
----------------------------------------------------------------------------------------------------------------------------------------------

### Category:  2.FS

+---------+
| Step Name                                             | N |  Time Avg |  Time Min |  Time Max | % Time | GC Alloc Avg | GC Alloc Min | GC Alloc Max |
|-------------------------------------------------------|---|-----------|-----------|-----------|--------|--------------|--------------|--------------|
| Upload 3 64 kb files for 200 gamers in a transaction. | 3 | 353,03 ms | 313,59 ms | 378,38 ms |   71,9 |    480,30 MB |    479,02 MB |    481,54 MB |
| Read 256 bytes chunk from random file positions       | 3 |  14,35 ms |   6,59 ms |  21,65 ms |    2,9 |      6,97 MB |      6,86 MB |      7,07 MB |
| Retrieve file&tags from users                         | 3 |  17,26 ms |  10,77 ms |  27,36 ms |    3,5 |     17,07 MB |     16,99 MB |     17,16 MB |
| Read first 1KB from all gaming files                  | 3 | 106,18 ms |  69,57 ms | 152,27 ms |   21,6 |     66,26 MB |     63,64 MB |     68,87 MB |
-------------------------------------------------------------------------------------------------------------------------------------------------------

## SoloDB DETAILED BENCHMARK RESULTS

### Category:  1.General

+---------+
| Step Name                                    | N |  Time Avg |  Time Min |  Time Max | % Time | GC Alloc Avg | GC Alloc Min | GC Alloc Max |
|----------------------------------------------|---|-----------|-----------|-----------|--------|--------------|--------------|--------------|
| Inserting 10000 users                        | 3 | 440,52 ms | 371,21 ms | 561,98 ms |   64,0 |     36,56 MB |     36,38 MB |     36,92 MB |
| Searching for gaming users.                  | 3 |  22,40 ms |   7,15 ms |  52,29 ms |    3,3 |    153,84 KB |    128,48 KB |    183,16 KB |
| Optimize                                     | 3 |   6,41 ms |   6,15 ms |   6,91 ms |    0,9 |     488,00 B |     488,00 B |     488,00 B |
| Update users with username starting with 'a' | 3 |   7,10 ms |   6,44 ms |   7,75 ms |    1,0 |      9,69 KB |      8,48 KB |     12,11 KB |
| Delete users with <= 2 categories            | 3 | 188,62 ms | 181,52 ms | 195,94 ms |   27,4 |     10,76 MB |     10,50 MB |     11,04 MB |
| Paginated query (page 3 of 50 users)         | 3 | 453,30 μs | 280,00 μs | 800,00 μs |    0,1 |     16,40 KB |     15,86 KB |     17,40 KB |
| Complex query with multiple conditions       | 3 |   1,08 ms | 770,00 μs |   1,59 ms |    0,2 |     24,01 KB |     22,68 KB |     26,67 KB |
| Count users by username first letter         | 3 |  21,50 ms |  15,40 ms |  33,63 ms |    3,1 |     56,05 KB |     45,83 KB |     74,60 KB |
----------------------------------------------------------------------------------------------------------------------------------------------

### Category:  2.FS

+---------+
| Step Name                                             | N | Time Avg |  Time Min | Time Max | % Time | GC Alloc Avg | GC Alloc Min | GC Alloc Max |
|-------------------------------------------------------|---|----------|-----------|----------|--------|--------------|--------------|--------------|
| Upload 3 64 kb files for 200 gamers in a transaction. | 3 |   1,33 s |    1,21 s |   1,55 s |   96,7 |    231,23 MB |    230,85 MB |    231,91 MB |
| Read 256 bytes chunk from random file positions       | 3 |  8,46 ms |   6,24 ms | 12,77 ms |    0,6 |      2,20 MB |      2,17 MB |      2,23 MB |
| Retrieve file&tags from users                         | 3 |  2,46 ms | 300,00 μs |  6,78 ms |    0,2 |      6,99 KB |      6,95 KB |      7,08 KB |
| Read first 1KB from all gaming files                  | 3 | 34,50 ms |  29,48 ms | 43,79 ms |    2,5 |     17,37 MB |     17,05 MB |     17,56 MB |
-----------------------------------------------------------------------------------------------------------------------------------------------------