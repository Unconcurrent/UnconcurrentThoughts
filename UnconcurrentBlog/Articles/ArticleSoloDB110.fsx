module internal ArticleSoloDB110

open ArticleType
open Giraffe.ViewEngine
open System
open ArticleTools

let articleBodyMd = IO.File.ReadAllText (IO.Path.Combine [|__SOURCE_DIRECTORY__; "ArticleSoloDB110.md"|])

let private body = div [_style "font-size: 18px;text-align: justify;"] [
    markdown articleBodyMd
]

let internal get() = {
    Id = "SoloDB110"
    Date = DateTimeOffset(2026, 3, 6, 0, 0, 0, TimeSpan.Zero)
    Tags = ["SoloDB"; "Release"; "Relations"; "Transactions"]
    Title = "SoloDB 1.1: When Documents Grow Relations"
    Authors = [Authors.Unconcurrent]
    Description = "SoloDB 1.1 moves referential integrity, delete safety, nested rollback, and relation queries out of service cleanup code and into the database engine."
    Body = body
}
