module internal ArticleCommonUndergroundII

open ArticleType
open Giraffe.ViewEngine
open System
open System.IO
open ArticlePreprocessor

let private articleDirectory = Path.Combine(__SOURCE_DIRECTORY__, "CommonUndergroundII")

let private articleBody =
    File.ReadAllText(Path.Combine(articleDirectory, "ARTICLE.md"))
    |> stripLeadingH1
    |> rewriteAssetRoot "images" "common-underground-ii"
    |> collapseSoftLineBreaks

let private articleIntroduction, articleParts = articleBody |> splitArticleParts

let private articleStyles =
    style [] [rawText """
        .common-underground-ii-article :is(a, code) {
            overflow-wrap: anywhere;
        }

        .common-underground-ii-data-table {
            display: block;
            max-width: 100%;
            overflow-x: auto;
        }

        .common-underground-ii-data-table:focus-visible {
            outline: 2px solid var(--primary-color);
            outline-offset: 2px;
        }

        @media (max-width: 760px) {
            .logo {
                font-size: clamp(1.35rem, 7vw, 1.8rem);
                white-space: nowrap;
            }

            .welcome-title {
                font-size: clamp(2rem, 9vw, 2.5rem);
                line-height: 1.2;
                overflow-wrap: anywhere;
            }

            .common-underground-ii-article {
                text-align: left !important;
            }
        }
    """]

let private body =
    renderLongFormArticle
        "common-underground-ii"
        "abstract"
        true
        [articleStyles]
        articleIntroduction
        []
        articleParts

let internal staticFiles: (string * string) list = []

let internal get() = {
    Id = "CommonUndergroundII"
    Date = DateTimeOffset(2026, 7, 22, 0, 0, 0, TimeSpan.Zero)
    Tags = ["AI agents"; "LLMs"; "Simulation"; "Institutional reasoning"]
    Title = "Seven AI Governors Built a Second Civilization. Model Labels Did Not Predict Their Fate"
    Authors = [Authors.Unconcurrent]
    Description = "A forty-round seat-level comparison of Codex, Claude, and DeepSeek showing that objectives, leverage, memory, interfaces, and operator policy explained outcomes better than model labels."
    Body = body
}
