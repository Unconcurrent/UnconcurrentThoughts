module internal Rss

#if INTERACTIVE
#load "ArticleType.fsx"
#endif

open System
open System.Globalization
open System.IO
open System.Text
open System.Xml
open ArticleType

let private formatRssDate (date: DateTimeOffset) =
    date.UtcDateTime.ToString("R", CultureInfo.InvariantCulture)

let render
    (siteUrl: string)
    (title: string)
    (description: string)
    (articles: Article list)
    =
    let siteUrl = siteUrl.TrimEnd('/')

    let articles =
        articles
        |> List.sortByDescending (fun article -> article.Date)
        |> List.truncate 30

    let settings =
        XmlWriterSettings(
            Encoding = UTF8Encoding(false),
            Indent = true,
            OmitXmlDeclaration = false
        )

    use stream = new MemoryStream()
    use xml = XmlWriter.Create(stream, settings)

    xml.WriteStartDocument()
    xml.WriteStartElement("rss")
    xml.WriteAttributeString("version", "2.0")
    xml.WriteStartElement("channel")

    xml.WriteElementString("title", title)
    xml.WriteElementString("link", siteUrl + "/")
    xml.WriteElementString("description", description)
    xml.WriteElementString("language", "en")
    xml.WriteElementString("generator", "UnconcurrentBlog")

    match articles with
    | latest :: _ ->
        xml.WriteElementString("lastBuildDate", formatRssDate latest.Date)
    | [] ->
        ()

    for article in articles do
        let articleUrl =
            $"{siteUrl}/articles/{Uri.EscapeDataString(article.Id)}.html"

        xml.WriteStartElement("item")
        xml.WriteElementString("title", article.Title)
        xml.WriteElementString("link", articleUrl)
        xml.WriteElementString("description", article.Description)
        xml.WriteElementString("pubDate", formatRssDate article.Date)

        xml.WriteStartElement("guid")
        xml.WriteAttributeString("isPermaLink", "true")
        xml.WriteString(articleUrl)
        xml.WriteEndElement()

        for tag in article.Tags do
            xml.WriteElementString("category", tag)

        xml.WriteEndElement()

    xml.WriteEndElement()
    xml.WriteEndElement()
    xml.WriteEndDocument()
    xml.Flush()

    Encoding.UTF8.GetString(stream.ToArray())
