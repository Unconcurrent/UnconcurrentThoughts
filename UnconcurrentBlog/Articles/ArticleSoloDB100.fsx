module internal ArticleSoloDB100

open ArticleType
open Giraffe.ViewEngine
open System
open ArticleTools

let articleBodyMd = IO.File.ReadAllText (IO.Path.Combine [|__SOURCE_DIRECTORY__; "ArticleSoloDB100.md"|])

let private body = div [_style "font-size: 18px;text-align: justify;"] [
    markdown articleBodyMd
]


let internal get() = {
    Id = "SoloDB100"
    Date = DateTimeOffset(2026, 1, 31, 0, 0, 0, TimeSpan.Zero)
    Tags = ["SoloDB"; "Release"; "Events API"]
    Title = "SoloDB 1.0: Production Ready"
    Authors = [Authors.Unconcurrent]
    Description = "After two years of production deployment managing a 1.5TB database, SoloDB reaches version 1.0 with the new Events API."
    Body = body
}
