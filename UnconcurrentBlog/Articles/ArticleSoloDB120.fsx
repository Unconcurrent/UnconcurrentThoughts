module internal ArticleSoloDB120

open ArticleType
open Giraffe.ViewEngine
open System
open ArticleTools

let articleBodyMd = IO.File.ReadAllText (IO.Path.Combine [|__SOURCE_DIRECTORY__; "ArticleSoloDB120.md"|])

let private body = div [_style "font-size: 18px;text-align: justify;"] [
    markdown articleBodyMd
]

let internal get() = {
    Id = "SoloDB120"
    Date = DateTimeOffset(2026, 3, 23, 0, 0, 0, TimeSpan.Zero)
    Tags = ["SoloDB"; "Release"; "LINQ"; "Query Engine"]
    Title = "SoloDB 1.2: From String Builder to Typed SQL"
    Authors = [Authors.Unconcurrent]
    Description = "SoloDB 1.2.0 broadens LINQ support over DBRefMany relations. Supported queries stay inside SQLite. Unsupported shapes are rejected immediately."
    Body = body
}
