module internal ArticleTypeProvider

open ArticleType
open Giraffe.ViewEngine
open System
open ArticleTools

let private body = div [_style "font-size: 18px;"] [
    h2 [] [str "Abstract"]
    line
    text """
    This article demonstrates the implementation of a custom Type Provider in F#, developed as a 
    class library project using Visual Studio 2022. The Type Provider accepts a string parameter 
    at design time and performs case transformation—inverting uppercase characters to lowercase and 
    vice versa—making the string available at compile time rather than runtime. This example 
    illustrates the metaprogramming capabilities of F# Type Providers as compile-time code generators.
    """


    chapter "Prerequirements"    

    ul [] [
        li [] [str "Have "; a [_href "https://visualstudio.microsoft.com/vs/" ; _target "blank"] [str "Visual Studio 2022"]; str " installed."]
        li [] [str "Have all of the "; a [_href "https://learn.microsoft.com/en-us/visualstudio/ide/fsharp-visual-studio?view=vs-2022"; _target "blank"] [str "F# tooling"]; str " installed for VS 2022."]
    ]

    chapter "Materializing the Executable Project"
    
    ol [] [
        li [] [str "Launch Visual Studio 2022."]
        li [] [str """Select "Create a new project"."""]
        li [] [str """In the search bar, type "F# Console App" and select the template without the (.NET Framework) suffix(because I did not test with any other framework version, it may work, or not.). Click "Next"."""]
        li [] [str "Name your project (e.g., "; code [] [str "CaseChanging"]; str ")."]
        li [] [str """Select .NET 9 as the framework version, and click "Create"."""]
    ]
    text """
    This will create an F# executable project with a default Program.fs file, which you can run now to ensure that everything is set up correctly.
    """

    chapter "Materializing the Library Project for the Type Provider"

    ol [] [
        li [] [str """Right-click the solution in the Solution Explorer window."""]
        li [] [str """Click Add → New Project."""]
        li [] [str """In the search bar, type "F# Class Library" and select the template (again, not the .NET Framework version). Click "Next"."""]
        li [] [str """Give it a name (e.g., """; code [] [str "CaseChangingProvider"]; str """). Click "Next".""";]
        li [] [str "And, "; em [] [str "importantly, "]; str """set the .NET version to .NET Standard 2.0, and click "Create". This is crucial for Type Provider's in-process execution."""]
    ]
    text """
    This will create an F# class library project with a default Library.fs file. This library will contain our custom Type Provider.
    You will now add this library project as a dependency of the Executable Project.
    """

    chapter "Incorporating the Necessary SDK Files"
    text """
    Prior to implementing our custom Type Provider, we first need to include the F# Type Provider SDK helper files. 
    These files provide the foundational types and interfaces for building Type Providers and are named ProvidedTypes.fsi (signature file) and ProvidedTypes.fs (implementation file).
    """
    texts [
        str """
            To obtain ProvidedTypes.fsi and ProvidedTypes.fs, go to the 
        """; 
        a [_href "https://github.com/fsprojects/FSharp.TypeProviders.SDK"; _target "blank"] [code [] [str "fsprojects/FSharp.TypeProviders.SDK"]]
        str """ repository on GitHub, select the latest commit where the tests are passing (e.g., """; a [_href "https://github.com/fsprojects/FSharp.TypeProviders.SDK/tree/1f9af4aa63008b863ddc74d3af92524d86ecf8c9"; _target "blank"] [str "this one"];
        str "). Navigate to the "; code [] [str "src"]; str """ directory and manually copy or download these two files into your Library Project.
        Here is the correct order in which the project compiles, you can move them in the .fsproj file or by using the """; code [] [str "Alt+(up arrow)"]; str "|"; code [] [str "Alt+(down arrow)"]; str " shortcut combo in the Solution window."
        ol [] [
            li [] [str "ProvidedTypes.fsi"]
            li [] [str "ProvidedTypes.fs"]
            li [] [str "Library.fs(left from the project creation)"]
        ]
    ]

    chapter "Implementing the Provider"
    texts [
        str "Rename the "; code [] [str "Library.fs"]; str " file to "; code [] [str "CaseChangingProvider.fs"]
    ]

    text "Clear its contents and we will start fabricating the Type by setting up the namespace and imports:"
    
    fsharp "CaseChangingProvider.fs" """
        namespace MyProvider

        open System
        open System.Reflection
        open ProviderImplementation.ProvidedTypes // This comes from ProvidedTypes.fs/.fsi
        open Microsoft.FSharp.Core.CompilerServices

        // To make this assembly recognized as containing Type Providers.
        [<assembly: TypeProviderAssembly>] do ()
    """
    text "Next, we will define the core Type Provider class."

    fsharp "CaseChangingProvider.fs" """
    [<TypeProvider>]
    type public CaseChangingProvider(config: TypeProviderConfig) as this =
        inherit TypeProviderForNamespaces(config) // In many other online examples this is a parameterless constructor.

        let providerNamespace = "MyProvider.CaseChanger"
        let thisAssembly = Assembly.GetExecutingAssembly()

        // Logic to transform the input string, this function will modify the input string.
        let transformString (input: string) =
            let switch c = if Char.IsUpper c then Char.ToLower c else Char.ToUpper c
            String.Join("", input |> Seq.map switch)
    """

    text "We will implement a function to generate the type based on its given generic name."
    fsharp "CaseChangingProvider.fs(inside CaseChangingProvider)" ("""
        let buildGenericType (inputString: string) (dynamicTypeName: string) =
            let providedType = ProvidedTypeDefinition(
                thisAssembly,
                providerNamespace,
                dynamicTypeName,
                Some typeof<obj>
            )

            providedType.AddXmlDoc $"Provides compile-time case transformation for the input string: '{inputString}'."
            let transformedValue = transformString inputString

            // Add a static property to the generated type that holds the transformed string
            let staticValueProperty = ProvidedProperty(
                propertyName = "Value",
                propertyType = typeof<string>,
                isStatic = true,
                getterCode = (fun _args -> <@@ transformedValue @@>) // Quotation embeds the value at compile time
            )
            staticValueProperty.AddXmlDoc $\"\"\"
                The compile-time transformed string.
                Original input: '{inputString}'
                Transformed value: '{transformedValue}'
            \"\"\"
            providedType.AddMember staticValueProperty
            providedType
    """.Replace ("\\\"\\\"\\\"", "\"\"\""))

    text "The generic root type is subsequently defined as the following."

    fsharp "CaseChangingProvider.fs(inside CaseChangingProvider)" ("""
        let rootType = ProvidedTypeDefinition(
            thisAssembly,
            providerNamespace,
            "Transform", // The name of the type you'll use to access the provider, e.g., Transform<"MYSTRING">
            Some typeof<obj>
        )

        do rootType.AddXmlDoc $\"\"\"
            Case Changing Type Provider.
            Use this type with a static string parameter to get a transformed string.
            Example: type MyTransformed = {providerNamespace}.Transform<"exampleSTRING">
                     let value = MyTransformed.Value
        \"\"\"

        // Define the static parameters the 'Transform' type accepts.
        // In this case, a single string parameter named "input".
        let staticParameters =
            [ ProvidedStaticParameter("input", typeof<string>) ]

        do rootType.DefineStaticParameters(
            parameters = staticParameters,
            instantiationFunction = (fun typeNameWithArgs suppliedArguments ->
                match suppliedArguments with
                | [| :? string as actualInputString |] ->
                    buildGenericType actualInputString typeNameWithArgs
                | _ ->
                    // This will result in a compile-time error if the arguments are incorrect.
                    failwith "Invalid static arguments. This Type Provider expects a single string argument."
            )
        )
    """.Replace ("\\\"\\\"\\\"", "\"\"\""))

    text "Lastly, the type is registered under the designated namespace."
    fsharp "CaseChangingProvider.fs(inside CaseChangingProvider)" """
        do this.AddNamespace(providerNamespace, [rootType])
    """

    texts [
        str "Rebuild the solution using "; code [] [str "Ctr+Shift+B "]; str ". If file locking errors occur during rebuild—such as when "; code [] [str "Visual Studio"] ; str " locks the output DLL—you may need to restart "; code [] [str "Visual Studio 2022"] ; str " to release the handle.";
        br [];
        code [] [str """The process cannot access the file 'bin\Debug\netstandard2.0\TypeMaster.dll' because it is being used by another process. The file is locked by: "Microsoft Visual Studio 2022" """]
    ]

    chapter "Using the Provider"

    texts [str "Returning to the "; em [] [str "Executable Project"]; str ", inside Program.fs write:"]
    fsharp "Program.fs" """
        module Program

        // Opening the provider
        open MyProvider.CaseChanger

        [<EntryPoint>]
        let main args =
            printfn "%s %s" Transform<"HELLO">.Value Transform<"world!">.Value
            0
    """
    texts [
        code [] [str "Ctrl+F5"]
        str " and a window will pop up, displaying "
        code [] [str "hello WORLD!"]
    ]

    chapter "Ending Thoughts"
    text """
        This tutorial introduced the core concepts of F# Type Providers by implementing a compile-time string transformer.
        While the functionality is simple, it demonstrates the essential mechanisms—static parameters, compile-time type 
        generation, and embedding metadata—which can be extended toward more sophisticated compile-time integrations.
        A good exercise might be to build a runtime O(1) Fibonacci calculator using compile-time generated types.
    """
    texts [
        br []
        a [_href "https://github.com/Unconcurrent/UnconcurrentThoughts/discussions/1"] [str "Comments page."]
    ]
]

let internal get() = {
    Id = "TypeProviders"
    Date = DateTimeOffset(2025, 05, 19, 20, 0, 0, TimeSpan.Zero)
    Tags = ["F# Type Providers"; "F#"; "Tutorial"; "Compile-time"]
    Title = "Tutorial on Implementing a F# Type Provider At Home"
    Authors = [Authors.Unconcurrent]
    Description = "A detailed walkthrough on creating a compile-time F# Type Provider that transforms strings using case inversion using Visual Studio 2022."
    Body = body
}