module private Stylize

open System.Text.RegularExpressions
open System.IO
open System
open HtmlAgilityPack


let highlitherCss = File.ReadAllText (sprintf "%s/highlight.js/style.css" __SOURCE_DIRECTORY__)

let injectIntoHTML (css: string) (html: string) =
    // Parse HTML
    let htmlDoc = new HtmlDocument()
    htmlDoc.LoadHtml(html)
    
    // Extract class styles from CSS
    let classStyleMap =
        // Match class selectors and their style blocks
        let classRulePattern = @"\.([^\s\,\{]+)(?:[^\{]*)\{([^\}]*)\}"
        let matches = Regex.Matches(css, classRulePattern)
        
        [|
            for m in matches do
                let className = m.Groups.[1].Value.Trim()
                let styleBlock = m.Groups.[2].Value.Trim()
                
                // Extract individual style declarations
                let stylePattern = @"([^:]+):([^;]+);?"
                let styleMatches = Regex.Matches(styleBlock, stylePattern)
                
                let styles = 
                    [|
                        for sm in styleMatches do
                            let property = sm.Groups.[1].Value.Trim()
                            let value = sm.Groups.[2].Value.Trim()
                            property, value
                    |]
                
                className, styles
        |]
        |> Map.ofArray
    
    // Process elements with class attributes
    let elements = htmlDoc.DocumentNode.SelectNodes("//*[@class]")
    
    if elements <> null then
        for element in elements do
            let classAttr = element.GetAttributeValue("class", "")
            let classes = classAttr.Split([|' '|], StringSplitOptions.RemoveEmptyEntries)
            
            // Collect all applicable styles
            let styles = 
                classes
                |> Array.collect (fun cls -> 
                    if classStyleMap.ContainsKey(cls) then classStyleMap.[cls] else [||]
                )
                |> Array.groupBy fst  // Group by property name
                |> Array.map (fun (prop, values) -> 
                    // For duplicate properties, take the last one (CSS cascade rule)
                    prop, (values |> Array.last |> snd)
                )
            
            if styles.Length > 0 then
                // Create style string
                let styleString = 
                    styles
                    |> Array.map (fun (prop, value) -> $"{prop}: {value};")
                    |> String.concat " "
                
                // Append to existing style or create new
                let existingStyle = element.GetAttributeValue("style", "").Trim()
                let newStyle = 
                    if String.IsNullOrEmpty(existingStyle) then styleString
                    else if existingStyle.EndsWith(";") then $"{existingStyle} {styleString}"
                    else $"{existingStyle}; {styleString}"
                
                ignore (element.SetAttributeValue("style", newStyle))
    
    // Return the modified HTML
    htmlDoc.DocumentNode.OuterHtml