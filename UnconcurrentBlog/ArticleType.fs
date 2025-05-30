﻿module private ArticleType

open Authors
open Giraffe.ViewEngine
open System



// Article type to represent blog posts
type Article = {
    Id: string
    Date: DateTimeOffset
    Tags: string list
    Title: string
    Authors: Author list
    Description: string
    Body: XmlNode
}