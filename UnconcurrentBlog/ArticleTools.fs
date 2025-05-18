module private ArticleTools
open ArticleType
open Giraffe.ViewEngine
open System

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