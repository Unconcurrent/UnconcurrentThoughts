module internal DefaultStyle

#if INTERACTIVE
#load "Minify.fsx"
#endif

open Giraffe.ViewEngine
open System
open System.IO

// Define some helper values for consistent styling
let internal primaryColor = "#378bba"
let internal secondaryColor = "#30b9db"
let internal accentColor = "#267093"
let internal backgroundColor = "#f8f9fa"
let internal textColor = "#333"
let internal lightGray = "#e9ecef"
let internal mediumGray = "#ced4da"
let internal darkGray = "#6c757d"

let internal webIconBase64 = File.ReadAllBytes(__SOURCE_DIRECTORY__ + "/webIcon.svg") |> Convert.ToBase64String
let internal fontBase64 = File.ReadAllBytes(__SOURCE_DIRECTORY__ + "/fonts/Slabo27px-Regular.woff2") |> Convert.ToBase64String

let internal defaultMetas = [
    meta [ _charset "UTF-8" ]
    meta [ _name "viewport"; _content "width=device-width,initial-scale=1.0,minimum-scale=1.0,user-scalable=yes" ]
    meta [ _name "robots"; _content "index,follow" ]
    meta [ _name "author"; _content "Unconcurrent" ]
    link [_rel "icon"; _href (sprintf "data:image/svg+xml;base64,%s" webIconBase64); _type "image/svg+xml"]
    let fontUrl = $"data:data:font/woff2;charset=utf-8;base64,{fontBase64}"// "/fonts/Slabo27px-Regular.ttf"
    // force to wait for the font to load before rendering.
    // link [_rel "preload"; _href fontUrl; XmlAttribute.KeyValue("as", "font"); XmlAttribute.KeyValue("crossorigin", "anonymous")]
    style [] [(sprintf """
            :root {
                --primary-color: %s;
                --secondary-color: %s;
                --accent-color: %s;
                --background-color: %s;
                --text-color: %s;
                --light-gray: %s;
                --medium-gray: %s;
                --dark-gray: %s;
            }

            @font-face {
                font-family: 'Slabo 27px';
                font-style: normal;
                font-weight: 400;
                font-display: swap;
                src: url(%s);
            }
                    
            * {
                margin: 0;
                box-sizing: border-box;
            }
                    
            body {
                background-color: var(--background-color);
                color: var(--text-color);
                line-height: 1.6;
                font-family: "Slabo 27px", serif;
                font-optical-sizing: auto;
            }
                    
            .container {
                width: 90%%;
                max-width: 1200px;
                margin: 0 auto;
                padding: 0 15px;
            }
                    
            /* Navigation */
            nav {
                background-color: white;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                padding: 1rem 0;
                top: 0;
                z-index: 100;
            }

            .code-block-name {
                background-color: #2a3c40;
            }
                    
            .nav-container {
                display: flex;
                justify-content: space-between;
                align-items: center;
            }
                    
            .logo {
                font-size: 1.8rem;
                font-weight: 700;
                color: var(--primary-color);
                text-decoration: none;
            }
                    
            .logo span {
                color: var(--secondary-color);
            }
                    
            .nav-links {
                display: flex;
                list-style: none;
            }
                    
            .nav-links li {
                margin-left: 2rem;
            }
                    
            .nav-links a {
                text-decoration: none;
                color: var(--text-color);
                font-weight: 500;
                transition: color 0.3s;
            }
                    
            .nav-links a:hover {
                color: var(--primary-color);
            }
                    
            .nav-links a.active {
                color: var(--primary-color);
                border-bottom: 2px solid var(--primary-color);
            }
                    
            /* Header */
            header {
                padding: 1rem;
                text-align: center;
                padding-bottom: 0;
            }
                    
            .welcome-title {
                font-size: 2.5rem;
                color: var(--primary-color);
                margin-bottom: 1rem;
            }
                    
            .blog-description {
                font-size: 1.2rem;
                color: var(--dark-gray);
                max-width: 800px;
                margin: 0 auto 2rem;
            }
                    
            /* Main Content */
            main {
                padding: 2rem 0;
            }
                    
            .articles {
                display: grid;
                grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
                gap: 2rem;
            }
                    
            .article-card {
                background-color: white;
                border-radius: 8px;
                overflow: hidden;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                transition: transform 0.1s, box-shadow 0.1s;
            }

            a.article-card {
                text-decoration: none;
                color: inherit;
                display: block;
            }
                    
            .article-card:hover {
                transform: translateY(-5px);
                box-shadow: 0 10px 15px rgba(0, 0, 0, 0.1);
            }
                    
            .article-content {
                padding: 1.5rem;
            }
                    
            .article-meta {
                display: flex;
                justify-content: space-between;
                margin-bottom: 0.8rem;
                font-size: 0.9rem;
                color: var(--dark-gray);
            }
                    
            .article-date {
                font-style: italic;
            }
                    
            .article-tags {
                display: flex;
                flex-wrap: wrap;
                gap: 0.5rem;
                margin-bottom: 1rem;
            }

            .article-authors a {
                padding: 0.3em;
                color: var(--accent-color);
            }

            .article-authors::before{
              content: "By "
            }
                    
            .tag {
                background-color: var(--light-gray);
                color: var(--accent-color);
                padding: 0.2rem 0.6rem;
                border-radius: 4px;
                font-size: 0.8rem;
                font-weight: 500;
            }
                    
            .article-title,
            .article-title h2 {
                font-size: 1.4rem;
                margin-bottom: 0.8rem;
                color: var(--accent-color);
                text-decoration: none;
            }
                    
            .article-description {
                color: var(--text-color);
                margin-bottom: 1rem;
                display: -webkit-box;
                -webkit-line-clamp: 3;
                -webkit-box-orient: vertical;
                overflow: hidden;
            }
                    
            .read-more {
                display: inline-block;
                color: var(--primary-color);
                font-weight: 500;
                text-decoration: none;
                transition: color 0.3s;
            }
                    
            .read-more:hover {
                color: var(--secondary-color);
            }
                    
            /* Footer */
            footer {
                background-color: white;
                padding: 2rem 0;
                margin-top: 3rem;
                border-top: 1px solid var(--light-gray);
            }
                    
            .footer-content {
                display: flex;
                justify-content: space-between;
                align-items: center;
            }
                    
            .copyright {
                color: var(--dark-gray);
            }
                    
            /* Responsive Design */
            @media (max-width: 768px) {
                .articles {
                    grid-template-columns: 1fr;
                }
                        
                .nav-container {
                    flex-direction: column;
                }
                        
                .nav-links {
                    margin-top: 1rem;
                }
                        
                .nav-links li {
                    margin-left: 1rem;
                    margin-right: 1rem;
                }
                        
                .footer-content {
                    flex-direction: column;
                    text-align: center;
                    gap: 1rem;
                }

                .hljs {
                    font-size: 0.8rem;
                    overflow-x: auto;
                  }
            }
        """ primaryColor secondaryColor accentColor backgroundColor textColor lightGray mediumGray darkGray fontUrl
    ) |> Minify.css |> rawText]
]