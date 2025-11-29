module internal ArticleSoloDBOrg

open ArticleType
open Giraffe.ViewEngine
open System
open ArticleTools

let articleBodyMd = IO.File.ReadAllText (IO.Path.Combine [|__SOURCE_DIRECTORY__; "ArticleSoloDBOrg.md"|])

let private body = div [_style "font-size: 18px;text-align: justify;"] [
    markdown articleBodyMd
]


let internal get() = {
    Id = "SoloDBOrg"
    Date = DateTimeOffset(2025, 11, 29, 0, 0, 0, TimeSpan.Zero)
    Tags = ["SoloDB"; "Announcement"]
    Title = "Introducing solodb.org"
    Authors = [Authors.Unconcurrent]
    Description = "SoloDB now has a dedicated website with documentation, examples, and getting started guides."
    Body = body
}
