﻿module internal ArticleSoloDBvsLiteDB

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
    Authors = [Authors.Unconcurrent]
    Description = "Benchmarking SoloDB and LiteDB: how different architectural approaches impact performance, memory usage, and developer experience."
    Body = body
}