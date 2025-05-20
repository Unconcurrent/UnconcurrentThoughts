module private WebBuilder

open ArticleType
open Giraffe.ViewEngine
open DefaultStyle
open System.IO

// Helper function to render a single tag
let private renderTag (tag: string) =
    span [ _class "tag" ] [ str tag ]

let private authors article =
    p [ _class "article-authors" ] [ for author in article.Authors do a (match author.Link with None -> [] | Some link -> [_href link; _target "blank"]) [str author.Name] ]

// Helper function to render a single article card
let private renderArticleCard (article: Article) =
    div [ _class "article-card"; ] [
        div [ _class "article-content" ] [
            div [ _class "article-meta" ] [
                span [ _class "article-date" ] [ strf "%iAD.%02i.%02i %02i:%02i" article.Date.Year article.Date.Month article.Date.Day article.Date.Hour article.Date.Minute ]
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
                        navLink "/about.html" "About"
                    ]
                ]
            ]
            yield! bodyItems

            // Footer
            footer [] [
                div [ _class "container footer-content" ] [
                    p [ _class "copyright" ] [ strf "© %i Unconcurrent. " System.DateTime.Now.Year; a [_href "/LICENSE.html"] [str "All rights reserved..."] ]
                    div [ _class "footer-links" ] [
                        a [ _href "https://github.com/Unconcurrent/UnconcurrentThoughts/discussions"; _target "blank" ] [ str "Have anything to say or ask?" ]
                    ]
                    div [ _class "footer-links" ] [
                        a [ _href "https://github.com/Unconcurrent/UnconcurrentThoughts/discussions" ] [ str "Contact" ]
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
            div [ _class "container" ] [
                article.Body
            ]
        ]
    ]


let private blogName = "Unconcurrent Thoughts"

let internal pages = [
    let directory (name: string) (pages: (string * XmlNode) list) = pages |> List.map(fun (fileName, p) -> $"{name}/{fileName}", p)

    page "index.html" [
        meta [ _name "description"; _content "Your trusted source for in-depth technical articles, news, and resources on real programming, provided for you by Unconcurrent." ]
        title [] [ str blogName ] 
    ] [
        // Header
        header [] [
            div [ _class "container" ] [
                h1 [ _class "welcome-title" ] [ strf "Welcome to %s" blogName ]
                p [ _class "blog-description" ] [
                    str "Your trusted source for in-depth technical articles, news, and resources on "; a [_href "https://www.pbm.com/~lindahl/real.programmers.html"] [str "real programming"]; str ". "
                ]
            ]
        ]
            
        // Main Content
        main [] [
            div [ _class "container" ] [
                div [ _class "articles" ] (Articles.allArticles |> List.map renderArticleCard)
            ]
        ]
    ]

    page "about.html" [ title [] [ strf "About %s" blogName ] ] [
        // Main Content for About Page
        main [] [
            div [ _class "container" ] [
                h1 [ _class "page-title" ] [ str $"About {blogName}" ]
                ArticleTools.line
                div [ _class "about-content" ] [
                    p [] [
                        str "Welcome to \"Unconcurrent Thoughts,\" a digital space dedicated to the pursuit of what we affectionately term "
                        a [ _href "https://www.pbm.com/~lindahl/real.programmers.html"; _target "_blank" ] [ str "\"real programming.\"" ]
                        str " This isn't about chasing the latest ephemeral trend or the shiniest new framework. Instead, it's a haven for exploring the foundational, the intricate, and the enduring aspects of software craftsmanship."
                    ]
                    ArticleTools.chapter "What Does \"Unconcurrent\" Mean?"
                    p [] [
                        str "The name \"Unconcurrent Thoughts\" reflects a desire to delve into topics that might swim against the prevailing current, or perhaps explore the depths beneath the surface-level chatter of the tech world. It’s about taking the time for deliberate, focused thinking on complex subjects, rather than just keeping pace with the concurrent rush of information. We believe there's immense value in understanding the 'why' and 'how' at a fundamental level, even if it means stepping aside from the mainstream."
                    ]
                    ArticleTools.chapter "Our Philosophy"
                    p [] [
                        str "This blog is born from a conviction that true mastery in programming comes from a deep understanding of principles, an appreciation for elegance and efficiency, and a willingness to engage with challenging concepts. We aim to provide in-depth technical articles, insightful tutorials, and considered resources. Whether it's dissecting the nuances of a language feature, as seen in our "
                        let articleLink = Articles.allArticles |> Seq.find(fun a -> a.Id = "TypeProviders") 
                        a [ _href $"/articles/{articleLink.Id}.html" ] [ str "F# Type Provider tutorial" ]
                        str ", or examining patterns that lead to robust and maintainable systems, our goal is to be a trusted source for those who share this passion."
                    ]
                    ArticleTools.chapter "What You'll Find Here"
                    p [] [
                        str "Expect to find content that encourages you to think, to build, and to understand. From detailed walkthroughs of creating compile-time mechanisms to discussions on software architecture and beyond, \"Unconcurrent Thoughts\" is for the curious mind, the dedicated practitioner, and the lifelong learner in the art and science of programming. We're less about quick fixes and more about fostering a deeper comprehension that stands the test of time."
                    ]
                    p [] [
                        str "Join us as we explore the less-trodden paths and celebrate the profound satisfaction that comes from truly understanding the tools of our trade."
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

        File.WriteAllText (filePath, RenderView.AsString.htmlDocument text)

    for (fileName, page) in pages do
        writeWww fileName page

    let filePath = Path.Combine (dirPath, "fonts/Slabo27px-Regular.ttf")
    let fileDir = Path.GetDirectoryName filePath
    if not (Directory.Exists fileDir) then
        ignore (Directory.CreateDirectory fileDir)

    File.Copy (Path.Combine(__SOURCE_DIRECTORY__, "fonts", "Slabo27px-Regular.ttf"), filePath)
    copyFile "LICENSE.md" (__SOURCE_DIRECTORY__ + "/..") dirPath
    let licenseMarkdown = File.ReadAllText (__SOURCE_DIRECTORY__ + "/../" + "LICENSE.md")
    File.WriteAllText (Path.Combine(dirPath, "LICENSE.html"), ArticleTools.renderMarkdown licenseMarkdown)