module internal ArticleCommonUndergroundII

open ArticleType
open Giraffe.ViewEngine
open System
open System.IO
open ArticlePreprocessor
open ArticleTools

let private articleDirectory = Path.Combine(__SOURCE_DIRECTORY__, "CommonUndergroundII")

let private articleBody =
    File.ReadAllText(Path.Combine(articleDirectory, "ARTICLE.md"))
    |> stripLeadingH1
    |> collapseSoftLineBreaks

let private body =
    div [ _class "article-text"; _style "font-size: 18px; text-align: justify;" ] [
        markdown articleBody
    ]

let internal get() = {
    Id = "CommonUndergroundII"
    Date = DateTimeOffset(2026, 7, 24, 0, 0, 0, TimeSpan.Zero)
    Tags = ["Multi-agent systems"; "Agentic drift"; "Human-in-the-loop"]
    Title = "Agentic Echo Chamber"
    Authors = [Authors.Unconcurrent]
    Description = "Seven AI agents built a government, echoed one another's errors, and revealed why long-running agent crews drift without a human."
    Body = body
}
