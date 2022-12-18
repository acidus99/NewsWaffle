using System;
using System.IO;
using Gemini.Cgi;

using NewsWaffle.Models;
namespace NewsWaffle.Cgi.Views
{
    internal class ArticleView : BaseView
    {
        public ArticleView(StreamWriter sw)
            : base(sw) { }

        public void RenderArticle(ContentPage articlePage)
        {
            Header(articlePage);
            ReadOptions(articlePage);
            ArticleBody(articlePage);
            RenderFooter(articlePage);
        }

        private void Header(ContentPage articlePage)
        {
            RenderTitle(articlePage.Meta.SiteName);
            Out.WriteLine($"## {articlePage.Meta.Title}");
            if (articlePage.Published != null)
            {
                Out.WriteLine($"Published: {articlePage.Published.Value.ToString("yyyy-MM-dd HH:mm")} GMT");
            }
            if (!string.IsNullOrEmpty(articlePage.Byline))
            {
                Out.WriteLine($"Byline: {articlePage.Byline}");
            }
            if (articlePage.WordCount > 0)
            {
                Out.WriteLine($"Length: {articlePage.WordCount} words (~{articlePage.TimeToRead.Minutes} minutes)");
            }
            if (articlePage.Meta.FeaturedImage != null)
            {
                Out.WriteLine($"=> {MediaRewriter.GetPath(articlePage.Meta.FeaturedImage)} Featured Image");
            }
        }

        private void ReadOptions(ContentPage articlePage)
        {
            if (articlePage.IsReadability)
            {
                Out.WriteLine($"=> {CgiPaths.ViewLinks(articlePage.Meta.SourceUrl)} View in Link Mode");
            }
        }

        private void ArticleBody(ContentPage articlePage)
        {
            Out.WriteLine();
            if (articlePage.IsReadability)
            {
                foreach (var item in articlePage.Content)
                {
                    Out.Write(item.Content);
                }
            }
            else
            {
                Out.WriteLine("Based on meta data, we thought this was a article, But we were unable to extract one. This might be a page of primarily links, like a home page or category page, or an oddly formatted page.");

                if (!string.IsNullOrEmpty(articlePage.Excerpt))
                {
                    Out.WriteLine("Excerpt:");
                    Out.WriteLine($">{articlePage.Excerpt}");
                }

                Out.WriteLine($"=> {CgiPaths.ViewRaw(articlePage.Meta.SourceUrl)} Try in Raw View?");
                Out.WriteLine($"=> {CgiPaths.ViewLinks(articlePage.Meta.SourceUrl)} Try in Link View?");
            }
        }
    }
}