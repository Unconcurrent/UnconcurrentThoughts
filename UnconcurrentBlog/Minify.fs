module private Minify

open System
open System.Text.RegularExpressions


/// Minifies CSS by removing comments, unnecessary whitespace, and performing basic optimizations.
let css (cssInput: string) =
    // Remove all comments
    let removeComments (css: string) =
        Regex.Replace(css, "/\\*[\\s\\S]*?\\*/", "")
    
    // Collapse all whitespace sequences to a single space
    let collapseWhitespace (css: string) =
        Regex.Replace(css, "\\s+", " ")
    
    // Remove unnecessary whitespace around various punctuation
    let removeUnnecessaryWhitespace (css: string) =
        css
        |> fun s -> Regex.Replace(s, " ?\\{ ?", "{")
        |> fun s -> Regex.Replace(s, " ?\\} ?", "}")
        |> fun s -> Regex.Replace(s, " ?; ?", ";")
        |> fun s -> Regex.Replace(s, " ?: ?", ":")
        |> fun s -> Regex.Replace(s, " ?, ?", ",")
        |> fun s -> Regex.Replace(s, " ?> ?", ">")
        |> fun s -> Regex.Replace(s, " ?\\+ ?", "+")
        |> fun s -> Regex.Replace(s, " ?\\~ ?", "~")
    
    // Remove the last semicolon in each declaration block
    let removeLastSemicolon (css: string) =
        Regex.Replace(css, ";\\}", "}")
    
    // Shorten hex color codes where possible (e.g., #ffffff to #fff)
    let shortenHexColors (css: string) =
        Regex.Replace(css, "#([0-9a-f])\\1([0-9a-f])\\2([0-9a-f])\\3", "#$1$2$3", RegexOptions.IgnoreCase)
    
    // Apply all transformations in sequence
    cssInput
    |> removeComments
    |> collapseWhitespace
    |> removeUnnecessaryWhitespace
    |> removeLastSemicolon
    |> shortenHexColors
    |> fun s -> s.Trim()

