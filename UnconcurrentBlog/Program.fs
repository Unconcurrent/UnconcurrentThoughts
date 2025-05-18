open System.IO

let private wwwPath = sprintf "%s/../www" __SOURCE_DIRECTORY__ |> Path.GetFullPath

[<EntryPoint>]
let main _args =
    Directory.EnumerateFiles wwwPath |> Seq.iter File.Delete
    Directory.EnumerateDirectories wwwPath |> Seq.iter (fun p -> Directory.Delete(p, true))
    
    WebBuilder.readerWebsiteInto wwwPath
    0