#if INTERACTIVE
#r "nuget: Giraffe.ViewEngine, 1.4.0"
#r "nuget: HtmlAgilityPack, 1.12.1"
#r "nuget: Jint, 4.2.2"
#r "nuget: Markdig, 0.41.2"
#load "WebBuilder.fsx"
#endif

open System.IO

let private wwwPath = sprintf "%s/../www" __SOURCE_DIRECTORY__ |> Path.GetFullPath


let run () =
    Directory.EnumerateFiles wwwPath |> Seq.iter File.Delete
    Directory.EnumerateDirectories wwwPath |> Seq.iter (fun p -> Directory.Delete(p, true))
    
    WebBuilder.readerWebsiteInto wwwPath

#if INTERACTIVE
run()
#else
[<EntryPoint>]
let rec main _args =
    run()
    0
#endif