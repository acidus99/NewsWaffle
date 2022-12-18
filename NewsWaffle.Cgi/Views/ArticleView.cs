using System;
using System.IO;
using Gemini.Cgi;

using NewsWaffle.Models;
namespace NewsWaffle.Cgi.Views
{
    internal class ArticleView
    {
        TextWriter Out;

        public ArticleView(TextWriter tw)
        {
            Out = tw;
        }

        public void RenderArticle(ContentPage articlePage)
        {
            Out.WriteLine($"# 🧇 NewsWaffle: {articlePage.Meta.SiteName}");
            if (articlePage.IsReadability)
            {
                RenderArticleSuccess(articlePage);
            }
            else
            {
                RenderArticleFailed(articlePage);
            }
        }

        private void RenderArticleSuccess(ContentPage articlePage)
        {
            Out.WriteLine($"## {articlePage.Meta.Title}");
            if (articlePage.Published != null)
            {
                Out.WriteLine($"Published: {articlePage.Published.Value.ToString("yyyy-MM-dd HH:mm")} GMT");
            }
            if (!string.IsNullOrEmpty(articlePage.Byline))
            {
                Out.WriteLine($"Byline: {articlePage.Byline}");
            }
            Out.WriteLine($"Length: {articlePage.WordCount} words (~{articlePage.TimeToRead.Minutes} minutes)");
            if (articlePage.Meta.FeaturedImage != null)
            {
                Out.WriteLine($"=> {MediaRewriter.GetPath(articlePage.Meta.FeaturedImage)} Featured Image");
            }
            Out.WriteLine($"=> {CgiPaths.ViewLinks(articlePage.Meta.SourceUrl)} Mode: Article View. Try in Link View?");
            Out.WriteLine();
            foreach (var item in articlePage.Content)
            {
                Out.Write(item.Content);
            }
        }

        private void RenderArticleFailed(ContentPage articlePage)
        {
            Out.WriteLine($"## {articlePage.Meta.Title}");
            if (articlePage.Meta.FeaturedImage != null)
            {
                Out.WriteLine($"=> {MediaRewriter.GetPath(articlePage.Meta.FeaturedImage)} Featured Image");
            }
            if (!string.IsNullOrEmpty(articlePage.Excerpt))
            {
                Out.WriteLine("Excerpt:");
                Out.WriteLine($">{articlePage.Excerpt}");
            }

            Out.WriteLine("Based on meta data, we thought this was a article, But we were unable to extract one. This might be a page of primarily links, like a home page or category page. this full article could not be properly parsed.");
            Out.WriteLine($"=> {CgiPaths.ViewRaw(articlePage.Meta.SourceUrl)} Try in Raw View?");
            Out.WriteLine($"=> {CgiPaths.ViewLinks(articlePage.Meta.SourceUrl)} Try in Link View?");
        }
    }
}
