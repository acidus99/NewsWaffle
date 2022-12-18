using System;
using System.IO;
using Gemini.Cgi;

using NewsWaffle.Models;
namespace NewsWaffle.Cgi.Views
{
    internal class FeedView : BaseView
    {
        private FeedPage FeedPage;

        public FeedView(StreamWriter sw, FeedPage feedPage)
            : base(sw)
        {
            FeedPage = feedPage;
        }

        public void RenderFeed()
        {
            RenderTitle();
            Header();
            ReadOptions();
            FeedBody();
            RenderFooter(FeedPage);
        }

        private void Header()
        {
            Out.WriteLine($"## {FeedPage.Meta.Title}");
            if (FeedPage.Meta.FeaturedImage != null)
            {
                Out.WriteLine($"=> {MediaRewriter.GetPath(FeedPage.Meta.FeaturedImage)} Featured Image");
            }
            if (FeedPage.Meta.Description.Length > 0)
            {
                Out.WriteLine($">{FeedPage.Meta.Description}");
            }
        }

        private void ReadOptions()
        {
            if (FeedPage.RootUrl.Length > 0)
            {
                Out.WriteLine($"=> {CgiPaths.ViewLinks(FeedPage.RootUrl)} Mode: RSS View. Try Link View?");
            }
        }

        private void FeedBody()
        {
            Out.WriteLine();
            Out.WriteLine($"### Feed Links: {FeedPage.Links.Count}");
            if (FeedPage.Links.Count > 0)
            {
                int counter = 0;
                foreach (var link in FeedPage.Links)
                {
                    counter++;
                    var published = link.HasPublished ? $"({link.TimeAgo})" : "";
                    Out.WriteLine($"=> {CgiPaths.ViewArticle(link.Url.AbsoluteUri)} {counter}. {link.Text} {published}");
                }
            }
            else
            {
                Out.WriteLine("This feed doesn't actually have any items in it. Unfortunately some sites have broken feeds with content.");
            }
        }
    }
}