using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

using OpenGraphNet;
using SmartReader;


using NewsWaffle.Models;

namespace NewsWaffle.Converter
{
    public class HtmlConverter
    {
        public HtmlConverter()
        {
        }

        private void AssignMetadata(AbstractPage page, OpenGraph metadata, string url, string html)
        {
            page.FeaturedImage = metadata.Image?.AbsoluteUri;
            page.OriginalSize = html.Length;
            page.Title = metadata.Title;
            page.SourceUrl = url;
        }

        private int CountWords(ContentItem content)
        {
            return content.Content.Split("\n").Where(x => !x.StartsWith("=> ")).Sum(x => CountWords(x));
        }
        private int CountWords(string s)
            => s.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;


        public AbstractPage ParseHtmlPage(string url, string html)
        {
            var metadata = OpenGraph.ParseHtml(html);

            if (metadata.Type == "website")
            {
                return ParseWebsite(url, html, metadata);
            }
            else if (metadata.Type == "article")
            {
                return ParseArticle(url, html, metadata);
            }
            return ParseWebsite(url, html, metadata);
        }

        public AbstractPage ForceArticle(string url, string html)
        {
            var metadata = OpenGraph.ParseHtml(html);
            return ParseArticle(url, html, metadata);
        }

        private ArticlePage ParseArticle(string url, string html, OpenGraph og)
        {
            var article = Reader.ParseArticle(url, html, null);
            var contentRoot = Preparer.PrepareHtml(article.Content);

            HtmlTagParser parser = new HtmlTagParser
            {
                ShouldRenderHyperlinks = false
            };
            parser.Parse(contentRoot);

            var contentItems = parser.GetItems();

            var parsedPage = new ArticlePage
            {
                Title = Sanitize(article.Title),
                FeaturedImage = article.FeaturedImage,
                SourceUrl = url,
                Content = contentItems,
                SimplifiedHtml = article.Content,
                Images = parser.Images,
                TimeToRead = article.TimeToRead,
                WordCount = CountWords(contentItems[0])
            };

            AssignMetadata(parsedPage, og, url, html);

            return parsedPage;
        }

        private string Sanitize(string s)
            => Regex.Replace(WebUtility.HtmlDecode(s),@"<[^>]*>", "");

        private HomePage ParseWebsite(string url, string html, OpenGraph og)
        {
            var contentRoot = Preparer.PrepareHtml(html);
            LinkExtractor extractor = new LinkExtractor(url);
            extractor.FindLinks(contentRoot);
            var homePage = new HomePage
            {
                Description = Sanitize(og.Metadata["og:description"].FirstOrDefault()?.Value ?? ""),
                ContentLinks = extractor.ContentLinks,
                NavigationLinks = extractor.NavigationLinks,
            };
            AssignMetadata(homePage, og, url, html);
            return homePage;
        }
    }
}
