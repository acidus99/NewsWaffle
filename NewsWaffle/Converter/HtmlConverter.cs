using System;
using System.IO;

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

        public AbstractPage ParseHtmlPage(string url, string html)
        {
            var og = OpenGraph.ParseHtml(html);

            if (og.Type == "website")
            {
                return ParseWebsite(url, html, og);
            }
            else if (og.Type == "article")
            {
                return ParseArticle(url, html, og);
            }
            return null;
        }

        private ArticlePage ParseArticle(string url, string html, OpenGraph og)
        {
            var article = Reader.ParseArticle(url, html, null);
            var contentRoot = Preparer.PrepareHtml(article.Content);

            HtmlTagParser parser = new HtmlTagParser();
            parser.Parse(contentRoot);

            var parsedPage = new ArticlePage
            {
                Title = article.Title,
                FeaturedImage = article.FeaturedImage,
                SourceUrl = url,
                Content = parser.GetItems(),
                SimplifiedHtml = article.Content
            };

            return parsedPage;
        }

        private HomePage ParseWebsite(string url, string html, OpenGraph og)
        {
            var contentRoot = Preparer.PrepareHtml(html);
            LinkExtractor extractor = new LinkExtractor(url);
            return new HomePage
            {
                Name = og.Title,
                Links = extractor.GetLinks(contentRoot)
            };
        }
    }
}
