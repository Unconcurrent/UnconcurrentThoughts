module private ArticleSoloDBvsLiteDB

open ArticleType
open Giraffe.ViewEngine
open System
open ArticleTools

let articleBodyMd = IO.File.ReadAllText (IO.Path.Combine [|__SOURCE_DIRECTORY__; "ArticleSoloDBvsLiteDB.md"|])

let private body = div [_style "font-size: 18px;text-align: justify;"] [
    markdown articleBodyMd
]
    

let internal get() = {
    Id = "SoloDBvsLiteDB"
    Date = DateTimeOffset(2025, 06, 08, 0, 0, 0, TimeSpan.Zero)
    Tags = ["SoloDB"; "LiteDB"; "Comparison"; "Showcase"]
    Title = "SoloDB vs LiteDB: A Performance and Usability Deep Dive"
    Description = "An in-depth comparison of SoloDB and LiteDB, showcasing SoloDB’s faster query performance, superior DX, and dramatic memory savings."
    Body = body
}