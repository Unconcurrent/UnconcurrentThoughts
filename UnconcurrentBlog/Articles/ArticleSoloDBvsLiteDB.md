## Preface

I'm introducing SoloDB - an embedded database that delivers ACID compliance, NoSQL flexibility, and SQL compatibility through [SQLite's JSONB](https://sqlite.org/jsonb.html) format. It offers an alternative approach to .NET document databases.

Let me share my experience and show you why SoloDB might be the better choice for your next project.

## Why was it created

SoloDB was created to simultaneously address these problems: NoSQL flexibility, SQL compatibility, full filesystem support, and strong ACID guarantees.
While excellent databases like LiteDB already existed, each had different design tradeoffs.

## The Comparison

I ran comprehensive benchmarks comparing SoloDB against LiteDB, while trying to make the tests as optimized as possible for both contenders. The results are appended to this document.

### The Improvements

In a "GroupBy and count users by username's first letter" query, SoloDB is **57% faster** than LiteDB (21,50 ms vs 50,08 ms). But it's not just about raw speed - it's about ease of achieving these speeds.

LiteDB uses its [ILiteQueryable](https://github.com/litedb-org/LiteDB/blob/master/LiteDB/Client/Database/ILiteQueryable.cs#L8) interface with a custom query language, while SoloDB takes a different approach with standard [IQueryable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.linq.iqueryable-1) support.

And the most dramatic improvement was memory allocation. For the same query, LiteDB allocates **30,37 MB** while SoloDB uses just **56,05 KB** - that's a **99.8% reduction**!

These performance improvements are primarily due to SQLite's highly optimized query engine and mature storage architecture that SoloDB builds upon.

## The GroupBy Comparison

In a GroupBy query, SoloDB outperforms LiteDB, here is how it is done:

In SoloDB, the test is implemented with LINQ:

```csharp
var letterCounts = users
    .GroupBy(x => x.Username[0])
    .Select(x => new { Key = x.Key, Count = x.Count() })
    .ToDictionary(k => k.Key.ToString(), e => e.Count);
```

But in LiteDB it is implemented using their query language(for speed):

```csharp
var letterCounts = users
    .Query()
    .GroupBy(BsonExpression.Create("SUBSTRING(Username, 0, 1)"))
    .Select(BsonExpression.Create("{key: @key, count: COUNT(*)}"))
    .ToEnumerable()
    .ToDictionary(k => k["key"].AsString, e => e["count"].AsInt64);
```

LiteDB's query language offers powerful capabilities, though it requires learning syntax that differs from standard LINQ patterns.

## IQueryable Support

SoloDB, by contrast, provides standard `IQueryable` support that translates directly to SQL with:

- Full IntelliSense integration
- No need to learn a query language
- Compile-time type safety
- Predictable performance characteristics
- No need for *.AsEnumerable()* workarounds.

A query like `users.Where(x => x.Username.StartsWith("a"))` compiles to SQL, taking advantage of indexes when available, which LiteDB also does.

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

SoloDB's foundation takes a different approach by building directly on SQLite's capabilities. This gives us:

1. **Battle-tested reliability**: Used in everything from browsers to spacecraft, SQLite is the most battle-tested database in existence.
2. **Proper transactions**: Full ACID compliance with SQLite's proven transaction system
3. **Efficient storage**: JSONB provides compact storage with fast access
4. **Compressed Filesystem**: The virtual filesystem is transparently compressed using [Snappy](https://github.com/brantburnett/Snappier).
5. **SQL compatibility**: You can use raw SQL when needed.
6. **Better tooling**: Any SQLite tool can inspect your database.

I claim no credit for these features — SoloDB simply stands on the shoulders of the giants behind SQLite.

## Different File System Integration

Both databases support file storage, but SoloDB implements a hierarchical virtual filesystem that offers:

1. Retrieve entities by Unix-style paths.
2. List files and directories using a built-in index.
3. Recursively enumerate contents of any folder.
4. Write, move, and replace files & directories transactionally.
5. Open or copy files by path, as streams or simple read calls.
6. Attach and query metadata for richer file context.
7. Perform indexed hash-based file lookups.

## Performance

By leveraging SQLite's mature architecture, we can obtain the following results:

### Operations Where SoloDB Excels

- **Inserting 10,000 users**:
  - SoloDB is 29,3% faster, with 94,9% less memory allocation (622,98 ms -> 440,52 ms)
- **Reading random file chunks**:
  - 41,0% faster, and 68,4% less memory used (14,35 ms -> 8,46 ms)
- **Complex queries**:
  - 95,7% faster, and 99,9% less memory (24,84 ms -> 1,08 ms)

### Cases Where LiteDB Has the Edge

- **Searching within array properties:**
  LiteDB supports indexes on Properties that are arrays; SoloDB does not — yet.

- **File write operations:**
  LiteDB is faster when writing files due to its minimal overhead.

- **Very simple queries:**
  For ultra-fast, cache-resident queries, LiteDB may outperform SoloDB due to smaller query to engine pipeline.

<br>

LiteDB excels at file write operations without the overhead of:

- Storing creation and modification timestamps.
- Updating all parent directory timestamps.
- Compressing file chunks with [Snappy](https://github.com/brantburnett/Snappier).
- Hashing contents for integrity and fast lookup.
- Enforcing valid path rules and length limits.
- Preventing illegal names (e.g., `..`, or malformed paths).

If these features are not required, then LiteDB is better.

## The Developer Experience

SoloDB provides a different developer experience:

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
    // Complex projection methods like SelectMany are supported — 
    // they translate cleanly into SQL when used with simple selectors.
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

Just C# that reads like C#.

## Advanced Features

Here are some modern features that were not highlighted yet.

1. **Custom ID generators**: Implement your own ID strategy for `Int64`, `String`, or `Guid` — such as Version 7 GUIDs.
2. **Attribute-based indexing**: Use `[Indexed]` attributes for simplicity, or create indexes manually via code.
3. **Hash-based file lookup**: All files are SHA-1 content-hashed.
4. **Auto optimization**: On startup, SoloDB runs ```PRAGMA optimize;```, enabling SQLite to gather fresh statistics and optimize future queries.
5. **Object inheritance support**: Collections support base and derived types.
                                   An `Animal` collection can store a `Cat`, and type checks like ```animal.GetType() == typeof(Cat)``` are fully supported in queries.
6. **[Dapper](https://github.com/DapperLib/Dapper)-like interoperability methods**: Execute, Query\<T\>, QueryFirst\<T\>, ...

## The Verdict

The benchmarks show that SoloDB's approach of building on SQLite's foundation delivers strong performance benefits for many common operations. This is largely thanks to SQLite's decades of optimization work.

Both databases are excellent choices with different architectural philosophies:

**SoloDB excels when you need:**
- Standard LINQ support without learning new syntax
- SQLite's proven ACID guarantees  
- Integration with existing SQLite tooling
- Lower GC memory usage for complex queries

**LiteDB remains excellent for:**
- Array property indexing (which SoloDB doesn't support yet)
- Faster file write operations
- Simpler deployment scenarios
- Projects already using LiteDB successfully


**Ready to choose?** Download [SoloDB](https://www.nuget.org/packages/SoloDB) if SQLite's foundation appeals to you, or stick with [LiteDB](https://www.nuget.org/packages/LiteDB) if it's already serving you well.

And if you don't like SoloDB?
Just *delete it* — and keep using your data directly with SQL.
At the end of the day, all the way down, it's just plain old SQLite.

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