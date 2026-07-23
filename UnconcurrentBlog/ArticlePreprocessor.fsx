module internal ArticlePreprocessor

open System
open System.Text.RegularExpressions
open Giraffe.ViewEngine
open ArticleTools

type internal ArticlePart = {
    Roman: string
    Title: string
    Markdown: string
}

let stripLeadingH1 (markdownText: string) =
    let normalized = markdownText.Replace("\r\n", "\n")
    if not (normalized.StartsWith("# ", StringComparison.Ordinal)) then
        normalized
    else
        match normalized.IndexOf('\n') with
        | -1 -> ""
        | firstLineEnd -> normalized.Substring(firstLineEnd + 1).TrimStart('\n')

let rewriteAssetRoot (sourceRoot: string) (outputRoot: string) (markdownText: string) =
    markdownText.Replace($"({sourceRoot}/", $"({outputRoot}/")

let addImageDimensions (dimensions: (string * int * int) list) (markdownText: string) =
    dimensions
    |> List.fold (fun rendered (source, width, height) ->
        let imagePattern = $"(!\\[[^\\]]*\\]\\({Regex.Escape(source)}\\))"
        Regex.Replace(
            rendered,
            imagePattern,
            $"$1{{width={width} height={height}}}")) markdownText

let addPartHeadingRules (markdownText: string) =
    markdownText.Replace("\r\n", "\n").Split('\n')
    |> Array.collect (fun line ->
        if Regex.IsMatch(line, "^# Part [IVX]+:") then
            [|line; ""; "@line"|]
        else
            [|line|])
    |> String.concat "\n"

let splitArticleParts (markdownText: string) =
    let normalized = markdownText.Replace("\r\n", "\n")
    let matches =
        Regex.Matches(normalized, "^# Part ([IVX]+): (.+)$", RegexOptions.Multiline)
        |> Seq.cast<Match>
        |> Seq.toArray

    if matches.Length <> 3 then
        failwithf "Expected exactly three article Parts, found %d." matches.Length

    let introduction = normalized.Substring(0, matches.[0].Index).TrimEnd()
    let parts =
        matches
        |> Array.mapi (fun index heading ->
            let bodyStart = heading.Index + heading.Length
            let bodyEnd =
                if index + 1 < matches.Length then matches.[index + 1].Index
                else normalized.Length
            {
                Roman = heading.Groups.[1].Value
                Title = heading.Groups.[2].Value
                Markdown = normalized.Substring(bodyStart, bodyEnd - bodyStart).Trim()
            })
        |> Array.toList

    introduction, parts

let renderPartSections (partNumber: int) (nextPartAnchor: string option) (markdownText: string) =
    let lines = markdownText.Replace("\r\n", "\n").Split('\n')
    let sectionLines =
        lines
        |> Array.indexed
        |> Array.filter (fun (_, line) -> Regex.IsMatch(line, "^## "))

    let sections =
        sectionLines
        |> Array.mapi (fun sectionIndex (lineIndex, _) ->
            lineIndex, sectionIndex, $"common-underground-section-{partNumber}-{sectionIndex + 1}")

    let sectionByLine =
        sections
        |> Array.map (fun (lineIndex, sectionIndex, anchor) -> lineIndex, (sectionIndex, anchor))
        |> Map.ofArray

    let sectionTarget sectionIndex =
        if sectionIndex + 1 < sections.Length then
            let _, _, nextAnchor = sections.[sectionIndex + 1]
            nextAnchor, "Skip to the next section →", "Skip →"
        else
            match nextPartAnchor with
            | Some nextPart -> nextPart, "Skip to the next Part →", "Next Part →"
            | None -> "abstract", "Back to the Abstract ↑", "Abstract ↑"

    let sectionReplacements =
        sections
        |> Array.map (fun (_, sectionIndex, anchor) ->
            let target, label, shortLabel = sectionTarget sectionIndex
            let anchorMarker = $"<!--cu-anchor-{partNumber}-{sectionIndex + 1}-->"
            let skipMarker = $"<!--cu-skip-{partNumber}-{sectionIndex + 1}-->"
            (
                anchorMarker,
                $"<div id=\"{anchor}\" class=\"common-underground-anchor\" aria-hidden=\"true\"></div>",
                skipMarker,
                $"<a class=\"common-underground-section-skip\" href=\"#{target}\" aria-label=\"{label}\"><span class=\"common-underground-section-skip-long\" aria-hidden=\"true\">{label}</span><span class=\"common-underground-section-skip-short\" aria-hidden=\"true\">{shortLabel}</span></a>"
            ))

    let marked =
        lines
        |> Array.mapi (fun lineIndex line ->
            match Map.tryFind lineIndex sectionByLine with
            | None -> [|line|]
            | Some(sectionIndex, _) ->
                let anchorMarker = $"<!--cu-anchor-{partNumber}-{sectionIndex + 1}-->"
                let skipMarker = $"<!--cu-skip-{partNumber}-{sectionIndex + 1}-->"
                [|anchorMarker; line; skipMarker|])
        |> Array.concat
        |> String.concat "\n"

    let placeSkipBesideHeading (html: string) (marker: string) (link: string) =
        let markerIndex = html.IndexOf(marker, StringComparison.Ordinal)
        if markerIndex < 0 then
            failwithf "Rendered subsection marker is missing: %s" marker

        let headingStart = html.LastIndexOf("<h2", markerIndex, StringComparison.Ordinal)
        let headingCloseStart = html.IndexOf("</h2>", headingStart, StringComparison.Ordinal)
        if headingStart < 0 || headingCloseStart < 0 then
            failwithf "Rendered subsection heading is missing before: %s" marker

        let headingEnd = headingCloseStart + "</h2>".Length
        let heading = html.Substring(headingStart, headingEnd - headingStart)
        let betweenHeadingAndMarker = html.Substring(headingEnd, markerIndex - headingEnd)
        html.Substring(0, headingStart)
        + $"<div class=\"common-underground-section-heading\">{heading}{link}</div>"
        + betweenHeadingAndMarker
        + html.Substring(markerIndex + marker.Length)

    marked
    |> markdown
    |> RenderView.AsString.htmlNode
    |> fun rendered ->
        sectionReplacements
        |> Array.fold (fun (html: string) (anchorMarker, anchor, _, _) ->
            html.Replace(anchorMarker, anchor, StringComparison.Ordinal)) rendered
    |> fun rendered ->
        sectionReplacements
        |> Array.fold (fun html (_, _, skipMarker, link) ->
            placeSkipBesideHeading html skipMarker link) rendered
    |> rawText

let collapseSoftLineBreaks (markdownText: string) =
    let isMatch (pattern: string) (line: string) = Regex.IsMatch(line, pattern)
    let isListItem (line: string) = isMatch "^\\s*(?:[-+*]|[0-9]+[.)])\\s+" line
    let isQuote (line: string) = isMatch "^\\s*>" line
    let quoteBody (line: string) = line.TrimStart().Substring(1).TrimStart()
    let isStructural (line: string) =
        let trimmed = line.TrimStart()
        isMatch "^\\s*#{1,6}\\s+" line
        || isListItem line
        || isQuote line
        || trimmed.StartsWith("|", StringComparison.Ordinal)
        || trimmed.StartsWith("![", StringComparison.Ordinal)
        || trimmed.StartsWith("<!--", StringComparison.Ordinal)
        || trimmed.StartsWith("<", StringComparison.Ordinal)
        || trimmed = "@line"
        || isMatch "^\\s*(?:-{3,}|_{3,}|\\*{3,})\\s*$" line

    let lines = markdownText.Replace("\r\n", "\n").Split('\n')
    let output = ResizeArray<string>()
    let mutable inFence = false

    for line in lines do
        let isFence = isMatch "^\\s*```" line
        let mutable joined = false
        if output.Count > 0 && not inFence && not isFence then
            let previous = output.[output.Count - 1]
            let bothQuotes =
                isQuote previous
                && isQuote line
                && not (String.IsNullOrWhiteSpace(quoteBody previous))
                && not (String.IsNullOrWhiteSpace(quoteBody line))
            let listContinuation = isListItem previous && not (isStructural line)
            let ordinaryContinuation =
                not (isStructural previous) && not (isStructural line)
            if not (String.IsNullOrWhiteSpace previous)
               && not (String.IsNullOrWhiteSpace line)
               && (bothQuotes || listContinuation || ordinaryContinuation) then
                let continuation =
                    if bothQuotes then
                        quoteBody line
                    else
                        line.TrimStart()
                output.[output.Count - 1] <- previous.TrimEnd() + " " + continuation
                joined <- true

        if not joined then
            output.Add line

        if isFence then
            inFence <- not inFence

    String.concat "\n" output

let renderWithEmbeds (embeds: (string * XmlNode) list) (markdownText: string) =
    let rec render (remaining: string) (available: (string * XmlNode) list) : XmlNode list =
        let next =
            available
            |> List.choose (fun ((marker, _) as embed) ->
                let index = remaining.IndexOf(marker, StringComparison.Ordinal)
                if index < 0 then None else Some(index, embed))
            |> List.sortBy fst
            |> List.tryHead

        match next with
        | None ->
            if String.IsNullOrWhiteSpace remaining then [] else [markdown remaining]
        | Some(index, (marker, content)) ->
            let before = remaining.Substring(0, index)
            let after = remaining.Substring(index + marker.Length)
            let prefix =
                if String.IsNullOrWhiteSpace before then [] else [markdown before]
            prefix
            @ [content]
            @ render after (available |> List.filter (fun (candidate, _) -> candidate <> marker))

    for marker, _ in embeds do
        let first = markdownText.IndexOf(marker, StringComparison.Ordinal)
        if first < 0 then
            failwithf "Article embed marker is missing: %s" marker
        if markdownText.IndexOf(marker, first + marker.Length, StringComparison.Ordinal) >= 0 then
            failwithf "Article embed marker is duplicated: %s" marker

    render markdownText embeds
