using System;
using System.IO;
using Gemini.Cgi;

using NewsWaffle.Models;
namespace NewsWaffle.Cgi.Views
{
    internal class LinksView : ArticleView
    {
        LinkPage LinkPage => (LinkPage)Page;

        public LinksView(StreamWriter sw, LinkPage page)
            : base(sw, page) { }

        protected override void Header()
        {
            if (LinkPage.Meta.FeaturedImage != null)
            {
                Out.WriteLine($"=> {MediaRewriter.GetPath(LinkPage.Meta.FeaturedImage)} Featured Image");
            }
            if (LinkPage.Meta.Description.Length > 0)
            {
                Out.WriteLine($">{LinkPage.Meta.Description}");
            }
        }

        protected override void ReadOptions()
        {
            Out.WriteLine($"=> {CgiPaths.ViewArticle(LinkPage.Meta.SourceUrl)} Mode: Link View. This doesn't appear to be an article. Force Article View?");
            if (LinkPage.HasFeed)
            {
                Out.WriteLine($"=> {CgiPaths.ViewFeed(LinkPage.FeedUrl)} RSS/Atom Feed detected. Click here for more accurate list of links");
            }
        }

        protected override void Body()
        {
            Out.WriteLine();
            int counter = 0;
            if (LinkPage.ContentLinks.Count > 0)
            {
                Out.WriteLine($"### Content Links: {LinkPage.ContentLinks.Count}");

                foreach (var link in LinkPage.ContentLinks)
                {
                    counter++;
                    Out.WriteLine($"=> {CgiPaths.ViewArticle(link.Url.AbsoluteUri)} {counter}. {link.Text}");
                }
            }
            if (LinkPage.NavigationLinks.Count > 0)
            {
                Out.WriteLine($"### Navigation Links: {LinkPage.NavigationLinks.Count}");
                counter = 0;
                foreach (var link in LinkPage.NavigationLinks)
                {
                    counter++;
                    Out.WriteLine($"=> {CgiPaths.ViewAuto(link.Url.AbsoluteUri)} {counter}. {link.Text}");
                }
            }
        }
    }
}