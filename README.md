# UnconcurrentBlog

A minimalist static blog generator built entirely in F#. It compiles Markdown articles into self-contained HTML pages with syntax highlighting, embedded fonts, and responsive styling.

## Features

- **Pure Static Generation** - No server required; outputs plain HTML files
- **Type-Safe HTML** - Uses Giraffe.ViewEngine DSL to prevent invalid markup
- **Server-Side Syntax Highlighting** - Runs highlight.js via Jint at build time
- **Self-Contained Output** - Fonts embedded as base64; no external requests
- **Responsive Design** - Mobile-friendly grid layout
- **Zero Client-Side JavaScript** - All processing happens at build time

## Technology Stack

| Component | Library |
|-----------|---------|
| Runtime | .NET 9.0 |
| HTML Generation | [Giraffe.ViewEngine](https://github.com/giraffe-fsharp/Giraffe.ViewEngine) |
| Markdown Parsing | [Markdig](https://github.com/xoofx/markdig) |
| HTML Manipulation | [HtmlAgilityPack](https://html-agility-pack.net/) |
| JS Runtime | [Jint](https://github.com/sebastienros/jint) |
| Syntax Highlighting | [highlight.js](https://highlightjs.org/) |

## Project Structure

```
UnconcurrentBlog/
├── UnconcurrentBlog/
│   ├── Program.fsx           # Entry point
│   ├── WebBuilder.fsx        # Page generation
│   ├── Articles.fsx          # Article registry
│   ├── ArticleType.fsx       # Article data model
│   ├── ArticleTools.fsx      # Markdown & code processing
│   ├── Authors.fsx           # Author definitions
│   ├── DefaultStyle.fsx      # CSS styling
│   ├── Stylize.fsx           # CSS utilities
│   ├── Minify.fsx            # CSS minification
│   ├── JS.fsx                # highlight.js integration
│   ├── Articles/             # Article content (.fsx + .md)
│   ├── fonts/                # Typography (Slabo 27px)
│   └── highlight.js/         # Syntax highlighting assets
├── www/                      # Generated static site (output)
└── .github/workflows/        # GitHub Pages deployment
```

## Building

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)

### Generate the Site

```bash
cd UnconcurrentBlog
dotnet fsi --use:Program.fsx --exec --optimize-
```

Or use the included batch file:

```bash
runInteractive.cmd
```

The static site will be generated in the `www/` directory.

## Article Creation

1. **Create the Markdown file** at `Articles/ArticleMyTitle.md`:

```markdown
## Introduction

Your article content here...

### Code Example

```fsharp
let hello = "world"
```
```

2. **Create the F# module** at `Articles/ArticleMyTitle.fsx`:

```fsharp
#if !INTERACTIVE
module internal ArticleMyTitle
#endif

#load "../ArticleType.fsx"
#load "../ArticleTools.fsx"
#load "../Authors.fsx"

open ArticleType
open Giraffe.ViewEngine
open System
open ArticleTools

let private articleBodyMd = IO.File.ReadAllText(__SOURCE_DIRECTORY__ + "/ArticleMyTitle.md")

let private body =
    div [_class "article-text"] [
        markdown articleBodyMd
    ]

let internal get() = {
    Id = "MyTitle"
    Date = DateTimeOffset(2025, 1, 15, 12, 0, 0, TimeSpan.Zero)
    Tags = ["F#"; "Tutorial"]
    Title = "My Article Title"
    Authors = [Authors.Unconcurrent]
    Description = "A brief description for SEO and previews."
    Body = body
}
```

3. **Register the article** in `Articles.fsx`:

```fsharp
let internal allArticles = [
    ArticleMyTitle.get()
    // ... other articles
]
```

4. **Rebuild** the site.

## Deployment

The site automatically deploys to GitHub Pages when you push to the `master` branch. The workflow is defined in `.github/workflows/static.yml`.

To deploy manually, copy the contents of the `www/` directory to any static hosting provider.

## Content Pipeline

```
Markdown (.md)
    │
    ▼
Markdig Parser
    │
    ▼
HtmlAgilityPack DOM
    ├── Syntax highlighting (Jint + highlight.js)
    ├── CSS class injection
    ├── Link target="_blank" attributes
    └── Code block styling
    │
    ▼
Giraffe.ViewEngine
    │
    ▼
Static HTML (.html)
```

## License

See [LICENSE.md](LICENSE.md) for details.
