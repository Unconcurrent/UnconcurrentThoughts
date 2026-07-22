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

let private body =
    renderLongFormArticle
        "common-underground"
        "abstract"
        false
        []
        articleIntroduction
        ["<!-- interactive: sim3-timelapse -->", timelapseFrame]
        articleParts

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
    Title = "I Gave Five AI Agents Empires to Govern"
    Authors = [Authors.Unconcurrent]
    Description = "Five AI agents and one human governed equal underground states across thirty-one rounds, building companies, courts, religious law, censorship, alliances, and an executable empire."
    Body = body
}
