using System;
using System.IO;
using Gemini.Cgi;

using NewsWaffle.Models;
namespace NewsWaffle.Cgi.Views
{
    internal class LinksView : BaseView
    {
        public LinksView(StreamWriter sw)
            : base(sw) { }

        public void RenderLinks(LinkPage linkPage)
        {
            RenderTitle(linkPage.Meta.SiteName);
            Header(linkPage);
            ReadOptions(linkPage);
            Links(linkPage);
            RenderFooter();
        }

        private void Header(LinkPage linkPage)
        {
            Out.WriteLine($"## {linkPage.Meta.Title}");
            if (linkPage.Meta.FeaturedImage != null)
            {
                Out.WriteLine($"=> {MediaRewriter.GetPath(linkPage.Meta.FeaturedImage)} Featured Image");
            }
            if (linkPage.Meta.Description.Length > 0)
            {
                Out.WriteLine($">{linkPage.Meta.Description}");
            }
        }

        private void ReadOptions(LinkPage linkPage)
        {
            Out.WriteLine($"=> {CgiPaths.ViewArticle(linkPage.Meta.SourceUrl)} Mode: Link View. This doesn't appear to be an article. Force Article View?");
            if (linkPage.HasFeed)
            {
                Out.WriteLine($"=> {CgiPaths.ViewFeed(linkPage.FeedUrl)} RSS/Atom Feed detected. Click here for more accurate list of links");
            }
        }

        private void Links(LinkPage linkPage)
        {
            Out.WriteLine();
            int counter = 0;
            if (linkPage.ContentLinks.Count > 0)
            {
                Out.WriteLine($"### Content Links: {linkPage.ContentLinks.Count}");

                foreach (var link in linkPage.ContentLinks)
                {
                    counter++;
                    Out.WriteLine($"=> {CgiPaths.ViewArticle(link.Url.AbsoluteUri)} {counter}. {link.Text}");
                }
            }
            if (linkPage.NavigationLinks.Count > 0)
            {
                Out.WriteLine($"### Navigation Links: {linkPage.NavigationLinks.Count}");
                counter = 0;
                foreach (var link in linkPage.NavigationLinks)
                {
                    counter++;
                    Out.WriteLine($"=> {CgiPaths.ViewAuto(link.Url.AbsoluteUri)} {counter}. {link.Text}");
                }
            }
        }
    }
}
