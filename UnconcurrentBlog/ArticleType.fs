module private ArticleType

open Giraffe.ViewEngine
open System



// Article type to represent blog posts
type Article = {
    Id: string
    Date: DateTimeOffset
    Tags: string list
    Title: string
    Description: string
    Body: XmlNode
}