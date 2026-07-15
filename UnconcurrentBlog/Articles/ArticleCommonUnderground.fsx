module internal ArticleCommonUnderground

open ArticleType
open Giraffe.ViewEngine
open System
open System.IO
open ArticlePreprocessor

let private articleDirectory = Path.Combine(__SOURCE_DIRECTORY__, "CommonUnderground")

let private articleBody =
    File.ReadAllText(Path.Combine(articleDirectory, "ARTICLE.md"))
    |> stripLeadingH1
    |> rewriteAssetRoot "images" "common-underground"
    |> addImageDimensions [
        "common-underground/Screenshot_20260706_130651.q40.avif", 2337, 884
        "common-underground/Screenshot_20260706_151651.q40.avif", 2329, 1211
        "common-underground/Screenshot_20260710_222611.q40.avif", 2028, 1096
    ]
    |> collapseSoftLineBreaks

let private articleIntroduction, articleParts = articleBody |> splitArticleParts

let private timelapseFrame =
    rawText """
        <div class="common-underground-timelapse-frame">
            <p class="common-underground-timelapse-loading" role="status">
                The interactive simulation timelapse will appear here.<br>
                Please wait while it loads.
            </p>
            <script>
                (() => {
                    const host = document.currentScript.parentElement;
                    let observer;
                    let scheduled = false;

                    host.fitTimelapse = frame => {
                        const doc = frame.contentDocument;
                        const component = doc?.querySelector(".sim3-component");
                        if (!doc || !component) return;

                        doc.documentElement.style.overflow = "hidden";
                        doc.body.style.margin = "0";
                        doc.body.style.overflow = "hidden";

                        const fit = () => {
                            frame.style.height = "1px";
                            const height = Math.ceil(Math.max(
                                doc.documentElement.scrollHeight,
                                doc.body.scrollHeight,
                                component.scrollHeight,
                                component.getBoundingClientRect().height
                            ));
                            frame.style.height = `${height}px`;
                            host.style.height = `${height}px`;
                            host.classList.add("is-loaded");
                        };

                        const scheduleFit = () => {
                            if (scheduled) return;
                            scheduled = true;
                            requestAnimationFrame(() => {
                                scheduled = false;
                                fit();
                            });
                        };

                        fit();
                        observer?.disconnect();
                        if ("ResizeObserver" in window) {
                            observer = new ResizeObserver(scheduleFit);
                            observer.observe(component);
                        }
                    };
                })();
            </script>
            <iframe
                class="common-underground-timelapse"
                src="common-underground/sim3-timelapse.html"
                title="Interactive underground simulation timelapse"
                loading="lazy"
                fetchpriority="low"
                scrolling="no"
                onload="this.parentElement.fitTimelapse(this)"></iframe>
            <noscript><style>
                .common-underground-timelapse-frame { height: 1400px !important; }
                .common-underground-timelapse { visibility: visible !important; }
                .common-underground-timelapse-loading { display: none !important; }
            </style></noscript>
        </div>
    """

let private articleStyles =
    style [] [rawText """
        .common-underground-article img {
            display: block;
            width: auto;
            max-width: 100%;
            height: auto;
            margin: 1.25rem auto 0.5rem;
        }

        .common-underground-timelapse-frame {
            position: relative;
            box-sizing: border-box;
            width: 100%;
            height: 1080px;
            margin: 1.5rem 0 0;
        }

        .common-underground-timelapse-loading {
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

        .common-underground-timelapse {
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

        .common-underground-timelapse-frame.is-loaded .common-underground-timelapse {
            visibility: visible;
        }

        .common-underground-timelapse-frame.is-loaded .common-underground-timelapse-loading {
            display: none;
        }

        .common-underground-part {
            --common-underground-part-overhang: clamp(1.25rem, 6vw, 3rem);
            box-sizing: border-box;
            width: calc(100% + var(--common-underground-part-overhang));
            max-width: calc(100vw - 0.5rem);
            margin: 2rem 0 2.5rem 50%;
            padding: 0;
            transform: translateX(-50%);
            border: 0;
            background: transparent;
        }

        .common-underground-part > summary {
            box-sizing: border-box;
            display: list-item;
            width: calc(100% - var(--common-underground-part-overhang));
            margin: 0 auto;
            padding: 1rem 0;
            cursor: pointer;
            font-weight: 700;
            text-align: left;
        }

        .common-underground-part > summary::marker {
            color: currentColor;
            font-size: 0.78em;
        }

        .common-underground-part > summary:hover,
        .common-underground-part > summary:focus-visible {
            text-decoration: underline;
            text-decoration-thickness: 1px;
            text-underline-offset: 0.2em;
        }

        .common-underground-part[open] > summary {
            border-bottom: 1px solid rgba(128, 128, 128, 0.32);
        }

        .common-underground-part-number {
            font-size: 0.8em;
            letter-spacing: 0.1em;
            margin-right: 0.75rem;
            opacity: 0.7;
            text-transform: uppercase;
            white-space: nowrap;
        }

        .common-underground-part-title {
            font-size: 1.22em;
        }

        .common-underground-part-content {
            box-sizing: border-box;
            width: calc(100% - var(--common-underground-part-overhang));
            margin: 0 auto;
            padding: 1rem 0 1.5rem;
        }

        .common-underground-anchor {
            display: block;
            position: relative;
            top: -1rem;
            visibility: hidden;
        }

        .common-underground-section-heading {
            display: grid;
            grid-template-columns: minmax(0, 1fr) auto;
            align-items: baseline;
            column-gap: 1rem;
        }

        .common-underground-section-heading > h2 {
            min-width: 0;
        }

        .common-underground-section-skip {
            align-self: baseline;
            font-size: 0.82em;
            font-style: italic;
            white-space: nowrap;
        }

        .common-underground-section-skip-short {
            display: none;
        }

        @media (max-width: 760px) {
            .common-underground-timelapse-frame {
                height: 900px;
            }

            .common-underground-section-heading {
                column-gap: 0.55rem;
            }

            .common-underground-section-skip-long {
                display: none;
            }

            .common-underground-section-skip-short {
                display: inline;
            }
        }
    """]

let private renderPart partIndex part =
    let partAnchor = $"common-underground-part-{part.Roman.ToLowerInvariant()}"
    let nextPartAnchor =
        articleParts
        |> List.tryItem (partIndex + 1)
        |> Option.map (fun next -> $"common-underground-part-{next.Roman.ToLowerInvariant()}")
    let attributes =
        [
            _class "common-underground-part"
            _id partAnchor
            attr "open" "open"
        ]
    details attributes [
        summary [] [
            span [ _class "common-underground-part-number" ] [str $"Part {part.Roman}"]
            span [ _class "common-underground-part-title" ] [str part.Title]
        ]
        div [ _class "common-underground-part-content" ] [
            renderPartSections (partIndex + 1) nextPartAnchor part.Markdown
        ]
    ]

let private body =
    div [ _class "article-text common-underground-article"; _style "font-size: 18px; text-align: justify;" ] [
        articleStyles
        yield! articleIntroduction
               |> renderWithEmbeds ["<!-- interactive: sim3-timelapse -->", timelapseFrame]
        yield! articleParts |> List.mapi renderPart
    ]

let internal staticFiles =
    let images =
        [
            "Screenshot_20260706_094137.q40.avif"
            "Screenshot_20260706_130651.q40.avif"
            "Screenshot_20260706_151651.q40.avif"
            "Screenshot_20260710_222611.q40.avif"
        ]
        |> List.map (fun fileName ->
            Path.Combine(articleDirectory, "images", fileName),
            Path.Combine("articles", "common-underground", fileName))
    images @ [
        Path.Combine(articleDirectory, "sim3-timelapse.fragment.html"),
        Path.Combine("articles", "common-underground", "sim3-timelapse.html")
    ]

let internal get() = {
    Id = "CommonUnderground"
    Date = DateTimeOffset(2026, 7, 15, 0, 0, 0, TimeSpan.Zero)
    Tags = ["AI agents"; "LLMs"; "Simulation"; "Institutional reasoning"]
    Title = "I Made 6 Agents Rebuild Civilization"
    Authors = [Authors.Unconcurrent]
    Description = "Five AI agents and one human governed equal underground states across thirty-one rounds, building companies, courts, religious law, censorship, alliances, and an executable empire."
    Body = body
}
