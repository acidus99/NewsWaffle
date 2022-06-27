using System;
using System.IO;
using SmartReader;

using NewsWaffle.Models;

namespace NewsWaffle.Converter
{
    public class HtmlConverter
    {
        private string RawHtml;
        private string SourceUrl;

        public HtmlConverter()
        {
        }

        public ParsedPage ParseHtmlPage(string url, string html)
        {
            RawHtml = html;
            SourceUrl = url;

            var article = SimplifyHtml(SourceUrl, RawHtml);
            return ParseArticle(article);
        }

        private Article SimplifyHtml(string url, string html)
            => Reader.ParseArticle(url, html, null);

        private ParsedPage ParseArticle(Article article)
        {
            File.WriteAllText("/Users/billy/tmp/out.html", article.Content);

            var contentRoot = Preparer.PrepareHtml(article.Content);

            HtmlTagParser parser = new HtmlTagParser();
            parser.Parse(contentRoot);

            var parsedPage = new ParsedPage
            {
                Title = article.Title,
                FeaturedImage = article.FeaturedImage,
                SourceUrl = this.SourceUrl,
                Content = parser.GetItems()
            };

            return parsedPage;
        }
    }
}
