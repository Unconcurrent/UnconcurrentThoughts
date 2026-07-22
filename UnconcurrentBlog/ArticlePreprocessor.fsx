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

let renderPartSectionsFor
    (cssNamespace: string)
    (topAnchor: string)
    (keyboardScrollableTables: bool)
    (partNumber: int)
    (nextPartAnchor: string option)
    (markdownText: string)
    =
    let lines = markdownText.Replace("\r\n", "\n").Split('\n')
    let sectionLines =
        lines
        |> Array.indexed
        |> Array.filter (fun (_, line) -> Regex.IsMatch(line, "^## "))

    let sections =
        sectionLines
        |> Array.mapi (fun sectionIndex (lineIndex, _) ->
            lineIndex, sectionIndex, $"{cssNamespace}-section-{partNumber}-{sectionIndex + 1}")

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
            | None -> topAnchor, "Back to the Abstract ↑", "Abstract ↑"

    let sectionReplacements =
        sections
        |> Array.map (fun (_, sectionIndex, anchor) ->
            let target, label, shortLabel = sectionTarget sectionIndex
            let anchorMarker = $"<!--cu-anchor-{partNumber}-{sectionIndex + 1}-->"
            let skipMarker = $"<!--cu-skip-{partNumber}-{sectionIndex + 1}-->"
            (
                anchorMarker,
                $"<div id=\"{anchor}\" class=\"{cssNamespace}-anchor\" aria-hidden=\"true\"></div>",
                skipMarker,
                $"<a class=\"{cssNamespace}-section-skip\" href=\"#{target}\" aria-label=\"{label}\"><span class=\"{cssNamespace}-section-skip-long\" aria-hidden=\"true\">{label}</span><span class=\"{cssNamespace}-section-skip-short\" aria-hidden=\"true\">{shortLabel}</span></a>"
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
        + $"<div class=\"{cssNamespace}-section-heading\">{heading}{link}</div>"
        + betweenHeadingAndMarker
        + html.Substring(markerIndex + marker.Length)

    marked
    |> markdown
    |> RenderView.AsString.htmlNode
    |> fun rendered ->
        if keyboardScrollableTables then
            rendered.Replace(
                "<table>",
                $"<table class=\"{cssNamespace}-data-table\" tabindex=\"0\">",
                StringComparison.Ordinal)
        else
            rendered
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

let private longFormArticleCss = """
        .__LONGFORM__-article img {
            display: block;
            width: auto;
            max-width: 100%;
            height: auto;
            margin: 1.25rem auto 0.5rem;
        }

        .__LONGFORM__-timelapse-frame {
            position: relative;
            box-sizing: border-box;
            width: 100%;
            height: 1080px;
            margin: 1.5rem 0 0;
        }

        .__LONGFORM__-timelapse-loading {
            position: absolute;
            inset: 0;
            display: grid;
            place-content: center;
            margin: 0;
            color: inherit;
            line-height: 1.5;
            text-align: center;
            opacity: 0.68;
        }

        .__LONGFORM__-timelapse {
            position: relative;
            z-index: 1;
            display: block;
            box-sizing: border-box;
            width: 100%;
            height: 100%;
            border: 0;
            background: transparent;
            visibility: hidden;
        }

        .__LONGFORM__-timelapse-frame.is-loaded .__LONGFORM__-timelapse {
            visibility: visible;
        }

        .__LONGFORM__-timelapse-frame.is-loaded .__LONGFORM__-timelapse-loading {
            display: none;
        }

        .__LONGFORM__-part {
            --__LONGFORM__-part-overhang: clamp(1.25rem, 6vw, 3rem);
            box-sizing: border-box;
            width: calc(100% + var(--__LONGFORM__-part-overhang));
            max-width: calc(100vw - 0.5rem);
            margin: 2rem 0 2.5rem 50%;
            padding: 0;
            transform: translateX(-50%);
            border: 0;
            background: transparent;
        }

        .__LONGFORM__-part > summary {
            box-sizing: border-box;
            display: list-item;
            width: calc(100% - var(--__LONGFORM__-part-overhang));
            margin: 0 auto;
            padding: 1rem 0;
            cursor: pointer;
            font-weight: 700;
            text-align: left;
        }

        .__LONGFORM__-part > summary::marker {
            color: currentColor;
            font-size: 0.78em;
        }

        .__LONGFORM__-part > summary:hover,
        .__LONGFORM__-part > summary:focus-visible {
            text-decoration: underline;
            text-decoration-thickness: 1px;
            text-underline-offset: 0.2em;
        }

        .__LONGFORM__-part[open] > summary {
            border-bottom: 1px solid rgba(128, 128, 128, 0.32);
        }

        .__LONGFORM__-part-number {
            font-size: 0.8em;
            letter-spacing: 0.1em;
            margin-right: 0.75rem;
            opacity: 0.7;
            text-transform: uppercase;
            white-space: nowrap;
        }

        .__LONGFORM__-part-title {
            font-size: 1.22em;
        }

        .__LONGFORM__-part-content {
            box-sizing: border-box;
            width: calc(100% - var(--__LONGFORM__-part-overhang));
            margin: 0 auto;
            padding: 1rem 0 1.5rem;
        }

        .__LONGFORM__-anchor {
            display: block;
            position: relative;
            top: -1rem;
            visibility: hidden;
        }

        .__LONGFORM__-section-heading {
            display: grid;
            grid-template-columns: minmax(0, 1fr) auto;
            align-items: baseline;
            column-gap: 1rem;
        }

        .__LONGFORM__-section-heading > h2 {
            min-width: 0;
        }

        .__LONGFORM__-section-skip {
            align-self: baseline;
            font-size: 0.82em;
            font-style: italic;
            white-space: nowrap;
        }

        .__LONGFORM__-section-skip-short {
            display: none;
        }

        @media (max-width: 760px) {
            .__LONGFORM__-timelapse-frame {
                height: 900px;
            }

            .__LONGFORM__-section-heading {
                column-gap: 0.55rem;
            }

            .__LONGFORM__-section-skip-long {
                display: none;
            }

            .__LONGFORM__-section-skip-short {
                display: inline;
            }
        }
    """

let private renderLongFormArticleStyles cssNamespace =
    longFormArticleCss.Replace("__LONGFORM__", cssNamespace)
    |> rawText
    |> List.singleton
    |> style []

let renderLongFormArticle
    (cssNamespace: string)
    (topAnchor: string)
    (keyboardScrollableTables: bool)
    (additionalBodyNodes: XmlNode list)
    (introduction: string)
    (embeds: (string * XmlNode) list)
    (parts: ArticlePart list)
    =
    let renderPart partIndex part =
        let partAnchor = $"{cssNamespace}-part-{part.Roman.ToLowerInvariant()}"
        let nextPartAnchor =
            parts
            |> List.tryItem (partIndex + 1)
            |> Option.map (fun next -> $"{cssNamespace}-part-{next.Roman.ToLowerInvariant()}")
        let attributes =
            [
                _class $"{cssNamespace}-part"
                _id partAnchor
                attr "open" "open"
            ]
        details attributes [
            summary [] [
                span [ _class $"{cssNamespace}-part-number" ] [str $"Part {part.Roman}"]
                span [ _class $"{cssNamespace}-part-title" ] [str part.Title]
            ]
            div [ _class $"{cssNamespace}-part-content" ] [
                renderPartSectionsFor
                    cssNamespace
                    topAnchor
                    keyboardScrollableTables
                    (partIndex + 1)
                    nextPartAnchor
                    part.Markdown
            ]
        ]

    div [ _class $"article-text {cssNamespace}-article"; _style "font-size: 18px; text-align: justify;" ] [
        renderLongFormArticleStyles cssNamespace
        yield! additionalBodyNodes
        yield! introduction |> renderWithEmbeds embeds
        yield! parts |> List.mapi renderPart
    ]
