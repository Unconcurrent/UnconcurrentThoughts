module internal WebBuilder

#if INTERACTIVE
#load "ArticleType.fsx"
#load "Articles.fsx"
#load "DefaultStyle.fsx"
#endif

open ArticleType
open Giraffe.ViewEngine
open DefaultStyle
open System.IO

// Helper function to render a single tag
let private renderTag (tag: string) =
    span [ _class "tag" ] [ str tag ]

let private authors article =
    p [ _class "article-authors" ] [ for author in article.Authors do a (match author.Link with None -> [] | Some link -> [_href link; _target "_blank"; _rel "noopener noreferrer"]) [str author.Name] ]

// Helper function to render a single article card
let private renderArticleCard (article: Article) =
    div [ _class "article-card"; ] [
        div [ _class "article-content" ] [
            div [ _class "article-meta" ] [
                span [ _class "article-date" ] [ strf "%i-%02i-%02i" article.Date.Year article.Date.Month article.Date.Day ]
            ]
            div [ _class "article-tags" ] (article.Tags |> List.map renderTag)
            a [ _href $"/articles/{article.Id}.html"; _class "article-title" ] [h2 [] [ str article.Title ]]
            authors article
            p [ _class "article-description" ] [ str article.Description ]
            a [ _href $"/articles/{article.Id}.html"] [span [ _class "read-more" ] [ str "Read more →" ]]
        ]
    ]

let private page (fileName: string) headItems bodyItems = 
    fileName, html [ _lang "en" ] [
        head [] [
            yield! defaultMetas
            yield! headItems
        ]
        body [] [
             // Navigation
            nav [] [
                div [ _class "container nav-container" ] [
                    a [ _href "/index.html"; _class "logo" ] [

                        str "Unconcurrent "
                        span [] [ str "Thoughts" ]
                    ]
                    ul [ _class "nav-links" ] [
                        let navLink href text = li [] [ a [ _href href; if href = fileName then _class "active" ] [ str text ] ]
                        navLink "/index.html" "Home"
                    ]
                ]
            ]
            yield! bodyItems

            // Footer
            footer [] [
                div [ _class "container footer-content" ] [
                    p [ _class "copyright" ] [ strf "© %i Unconcurrent" System.DateTime.Now.Year; ]

                    div [ _class "footer-links" ] [
                        a [ _href "https://github.com/Unconcurrent/UnconcurrentThoughts/discussions/2"; _target "_blank"; _rel "noopener"] [ str "GitHub discussions page" ]
                    ]

                    div [ _class "footer-links" ] [
                        a [_href "/LICENSE.html"] [str "Terms of Use"]
                    ]
                ]
            ]
        ]
    ]


let private renderArticle (article: Article) =
    page $"{article.Id}.html" [
        meta [ _name "description"; _content article.Description ]
        title [] [str article.Title]
    ] [
        header [] [
            div [ _class "container" ] [
                h1 [ _class "welcome-title" ] [ str article.Title ]
                div [_style "display: flex; flex-wrap: wrap; margin-bottom: 0.5rem; justify-content: space-evenly;"] [
                    div [ _class "article-tags" ] (article.Tags |> List.map renderTag)
                    authors article
                ]
            ]
        ]
        main [] [
            div [ _class "container"; _style "width: 100%;" ] [
                article.Body
            ]
        ]
    ]


let private blogName = "Unconcurrent Thoughts"

let internal pages = [
    let directory (name: string) (pages: (string * XmlNode) list) = pages |> List.map(fun (fileName, p) -> $"{name}/{fileName}", p)

    page "index.html" [
        meta [ _name "description"; _content "Unconcurrent's blog for in-depth technical articles, interesting libraries and general programming." ]
        title [] [ str blogName ]
    ] [
        // Header
        header [] [
            div [ _class "container" ] [
                p [ _class "blog-description" ] [
                    str "This is my blog for in-depth technical articles, interesting libraries and general programming."
                ]
            ]
        ]

        // Main Content
        main [] [
            div [ _class "container"; _style "width: 100%; padding: 0px;" ] [
                div [ _class "articles" ] (Articles.allArticles |> List.map renderArticleCard)
            ]
        ]
    ]

    let licenseMd = File.ReadAllText (Path.Combine(__SOURCE_DIRECTORY__, "..", "LICENSE.md"))
    page "LICENSE.html" [
        meta [ _name "description"; _content "Legal information for Unconcurrent Thoughts." ]
        title [] [ str "Legal - Unconcurrent Thoughts" ]
    ] [
        main [] [
            div [ _class "container" ] [
                article [ _style "max-width: 800px; margin: 0 auto; font-size: 18px; text-align: justify;" ] [
                    h1 [] [ str "Legal" ]

                    ArticleTools.markdown licenseMd

                    h2 [] [ str "Font License" ]
                    p [] [
                        str "This website uses "
                        strong [] [ str "Slabo 27px" ]
                        str " (Copyright © 2013, Tiro Typeworks Ltd), licensed under the "
                        a [ _href "https://openfontlicense.org"; _target "_blank"; _rel "noopener" ] [ str "SIL Open Font License, Version 1.1" ]
                        str "."
                    ]
                ]
            ]
        ]
    ]

    yield! directory "articles" [
        yield! Articles.allArticles |> List.map renderArticle
    ]
]

let private copyFile fileName sourceDir destDir =
    let s = Path.Combine (sourceDir,fileName)
    let d = Path.Combine (destDir,fileName)

    File.Copy(s, d)

let readerWebsiteInto (dirPath: string) =
    let writeWww fileName text =
        let filePath = Path.Combine (dirPath, fileName)
        let fileDir = Path.GetDirectoryName filePath
        if not (Directory.Exists fileDir) then
            ignore (Directory.CreateDirectory fileDir)

        if File.Exists filePath then
            failwithf "File rendered page already exists: %s" filePath

        File.WriteAllText (filePath, RenderView.AsString.htmlDocument text)

    for (fileName, page) in pages do
        writeWww fileName page

    let filePath = Path.Combine (dirPath, "fonts/Slabo27px-Regular.ttf")
    let fileDir = Path.GetDirectoryName filePath
    if not (Directory.Exists fileDir) then
        ignore (Directory.CreateDirectory fileDir)

    File.Copy (Path.Combine(__SOURCE_DIRECTORY__, "fonts", "Slabo27px-Regular.ttf"), filePath)