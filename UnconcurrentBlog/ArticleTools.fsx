module internal ArticleTools

#if INTERACTIVE
#load "JS.fsx"
#load "ArticleType.fsx"
#endif

open Markdig.Renderers
open Markdig.Renderers.Html
open Markdig.Syntax
open Giraffe.ViewEngine
open System
open System.Web
open Markdig
open HtmlAgilityPack

// Helper function to create a paragraph with justified text
let text (txt: string) = p [_style "text-align: justify;"] [str txt]

// Helper function to create a paragraph with a list of text elements, justified
let texts (txts) = p [_style "text-align: justify;"] txts

// Helper function to create a horizontal line
let line = hr [_style "opacity: 0.1;"]

// Helper function to format code blocks
let code a b = code ([_style "background-color: #e4ecee;"] @ a) b

// Function to remove common leading indentation from a multi-line string
let removeIndentation (text: string) =
    let lines = text.Split '\n'
    let lines = lines |> Array.map (fun l -> l.TrimEnd '\r')
    if lines.Length = 0 then "" else
    if lines.Length = 1 then lines.[0].Trim ' ' else
    match lines |> Array.tryFindIndex(fun l -> l |> Seq.exists (not << Char.IsWhiteSpace)) with
    | None -> ""
    | Some firstLineWithChars ->

    let firstLineIdentLen = lines.[firstLineWithChars].Length - (lines.[firstLineWithChars].TrimStart ' ').Length
    let lines = 
        lines 
        |> Array.map(fun l -> if l.Length <= firstLineIdentLen || String.IsNullOrWhiteSpace(l) then null else l.Substring firstLineIdentLen)
        |> Array.filter (isNull >> not)
    String.concat "\n" lines

let fsharp fileName sourceCode = 
    (rawText << JS.highlightFS fileName << removeIndentation) sourceCode

let chapter name = div [] [
    br []
    h2 [] [str name]
    line
]

let pipeline = MarkdownPipelineBuilder().UseAdvancedExtensions().UseSoftlineBreakAsHardlineBreak().Build();

let renderMarkdown mark =
    Markdown.ToHtml(markdown = mark, pipeline = pipeline)

let renderASCIIMarkdown mark =
    for c in mark do if c >= char 127 then failwithf "Non ASCII char found in markdown: '%c'." c
    Markdown.ToHtml(markdown = mark, pipeline = pipeline)

/// Returns the very next sibling that is an element (skipping text/comments), if any.
let private nextElement (node: HtmlNode) =
    let rec loop (n: HtmlNode) =
        if isNull n then None
        elif n.NodeType = HtmlNodeType.Element then Some n
        else loop n.NextSibling
    loop node.NextSibling

let private prevElement (node: HtmlNode) =
    let rec loop (n: HtmlNode) =
        if isNull n then None
        elif n.NodeType = HtmlNodeType.Element then Some n
        else loop n.PreviousSibling
    loop node.PreviousSibling

let markdown (mark: string) =
    let mark = mark.Replace("@line", "--------------------------------------------------------")
    let html = renderMarkdown mark

    let htmlDoc = HtmlAgilityPack.HtmlDocument()
    htmlDoc.LoadHtml html


    for codeElements in htmlDoc.DocumentNode.Descendants() |> Seq.filter(fun d -> d.Name = "pre" && not (d.HasClass "hljs") && d.HasChildNodes && d.FirstChild.Name = "code" && d.FirstChild.GetClasses() |> Seq.exists(fun c -> c.StartsWith "language-")) |> Seq.toList do
        let innerText = codeElements.FirstChild.InnerHtml
        let innerText = HttpUtility.HtmlDecode innerText
        let lang = codeElements.FirstChild.GetClasses() |> Seq.find(fun c -> c.StartsWith "language-")
        let lang = lang.Substring("language-".Length)
        let highTxt = JS.highlight lang innerText
        ignore (codeElements.ParentNode.ReplaceChild(HtmlAgilityPack.HtmlNode.CreateNode(highTxt),codeElements))

    // for h3 in htmlDoc.DocumentNode.Descendants() |> Seq.filter(fun d -> d.Name = "h3" && d.PreviousSibling <> null && d.PreviousSibling.Name <> "div") |> Seq.toList do
    //     ignore (h3.ParentNode.InsertBefore(HtmlAgilityPack.HtmlNode.CreateNode("<br>"), h3))

    for hr in htmlDoc.DocumentNode.Descendants() |> Seq.filter(fun d -> d.Name = "hr" && not(d.Attributes.Contains "style")) |> Seq.toList do
        hr.Attributes.Add("style", "opacity: 0.1;")

    for p in htmlDoc.DocumentNode.Descendants() |> Seq.filter(fun d -> d.Name = "p") |> Seq.toList do
        match nextElement p with
        | None -> ()
        | Some x when x.Name = "br" || x.Name = "pre" || x.Name = "ol" || x.Name = "ul" -> ()
        | Some _ ->
            // safe to insert <br>
            p.ParentNode.InsertAfter(HtmlNode.CreateNode("<br>"), p)
            |> ignore

    for h2 in htmlDoc.DocumentNode.Descendants() |> Seq.filter(fun d -> d.Name = "h2") |> Seq.toList do
        match prevElement h2 with
        | Some x when x.Name <> "div" && x.Name <> "br" ->
            ignore (h2.ParentNode.InsertBefore(HtmlAgilityPack.HtmlNode.CreateNode("<br>"), h2))
        | _ -> ()

        ignore (h2.ParentNode.InsertAfter(HtmlAgilityPack.HtmlNode.CreateNode("<hr style=\"opacity: 0.1;\">"), h2))


    for code in htmlDoc.DocumentNode.Descendants() |> Seq.filter(fun d -> d.Name = "code" && not (d.Attributes.Contains "style") && d.ParentNode.Name <> "pre") |> Seq.toList do
        code.Attributes.Add("style", "background-color: #d7d7d7; color: black;")


    for a in htmlDoc.DocumentNode.Descendants() |> Seq.filter(fun d -> d.Name = "a" && d.Attributes.Contains "href") |> Seq.toList do
        a.Attributes.Add("rel", "noopener noreferrer")
        a.Attributes.Add("target", "_blank")

    let html = htmlDoc.DocumentNode.InnerHtml
    rawText html

