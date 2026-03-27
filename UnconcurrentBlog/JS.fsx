module internal JS

#if INTERACTIVE
#load "Stylize.fsx"
#load "Minify.fsx"
#endif

open System.IO
open System.Web

let private highlightPath = sprintf "%s/highlight.js/" __SOURCE_DIRECTORY__

let private jsEngine = 
    let j = new Jint.Engine()

    // highlight.js
    let hPath = highlightPath + "highlight.js"
    let hText = File.ReadAllText hPath
    ignore (j.Execute (hText, hPath))

    for jsFile in Directory.EnumerateFiles (highlightPath + "languages", "*.js") do
        let jsText = File.ReadAllText jsFile
        ignore (j.Execute (jsText, jsFile))

    ignore(j.Execute """
    function highlightLang(text, lang) {
        return hljs.highlight(text, { language: lang }).value;
    }
    """)

    j


/// <summary>
/// Return HTML.
/// </summary>
/// <param name="code">F# source code</param>
let highlightFS (fileName: string) (code: string) =
    let value = jsEngine.Invoke ("highlightLang", code, "fsharp")
    if value.Type <> Jint.Runtime.Types.String then failwithf "The highlightLang func returned an invalid value type: %A" value.Type

    sprintf "<div class=\"code-block\"><pre class=\"hljs\"><span class=\"code-block-name\">%s</span><br/><code>%O</code></pre></div>" (HttpUtility.HtmlEncode fileName) value
    |> Stylize.injectIntoHTML Stylize.highlitherCss
    |> fun html -> html.Replace("width: 100%;", "").Replace("min-width: fit-content;", "").Replace("max-width: 100%;", "")

let highlight (lang: string) (code: string) =
    let value = jsEngine.Invoke ("highlightLang", code, lang)
    if value.Type <> Jint.Runtime.Types.String then failwithf "The highlightLang func returned an invalid value type: %A" value.Type

    sprintf "<div class=\"code-block\"><pre class=\"hljs\"><code>%O</code></pre></div>" value
    |> Stylize.injectIntoHTML Stylize.highlitherCss
    |> fun html -> html.Replace("width: 100%;", "").Replace("min-width: fit-content;", "").Replace("max-width: 100%;", "")
