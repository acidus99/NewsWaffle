using System.Net;
using Gemini.Cgi;
using NewsWaffle.Models;
using NewsWaffle.Cgi.Media;

namespace NewsWaffle.Cgi
{
    public static class RouteHandler
    {
        public static void View(CgiWrapper cgi)
        {
            if(!cgi.HasQuery)
            {
                cgi.Input("Enter URL of news site (e.g. 'https://www.wired.com'");
                return;
            }
            cgi.Success();
            cgi.Writer.WriteLine("# 🧇 NewsWaffle");

            var waffles = new YummyWaffles();
            var page = waffles.GetPage(cgi.Query);
            if (page == null)
            {
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine(waffles.ErrorMessage);
                return;
            }

            if (page is LinkPage)
            {
                RenderLinks(cgi, (LinkPage)page);
            }
            else
            {
                RenderArticle(cgi, (ContentPage)page);
            }

            Footer(cgi, page);
        }

        public static void Article(CgiWrapper cgi)
        {
            if (!cgi.HasQuery)
            {
                cgi.Input("Enter URL of news site (e.g. 'https://www.wired.com'");
                return;
            }
            cgi.Success();
            cgi.Writer.WriteLine("# 🧇 NewsWaffle");

            var waffles = new YummyWaffles();
            var page = waffles.GetContentPage(cgi.Query);

            if (page == null)
            {
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine(waffles.ErrorMessage);
                return;
            }
            RenderArticle(cgi, page);
            Footer(cgi, page);
        }

        public static void Links(CgiWrapper cgi)
        {
            if (!cgi.HasQuery)
            {
                cgi.Input("Enter URL of news site (e.g. 'https://www.wired.com'");
                return;
            }
            cgi.Success();
            cgi.Writer.WriteLine("# 🧇 NewsWaffle");

            var waffles = new YummyWaffles();
            var page = waffles.GetLinkPage(cgi.Query);

            if (page == null)
            {
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine(waffles.ErrorMessage);
                return;
            }
            RenderLinks(cgi, page);
            Footer(cgi, page);
        }


        public static void Feed(CgiWrapper cgi)
        {
            if (!cgi.HasQuery)
            {
                cgi.Redirect(CgiPaths.BasePath);
                return;
            }
            cgi.Success();
            cgi.Writer.WriteLine("# 🧇 NewsWaffle");

            var waffles = new YummyWaffles();
            var feedPage = waffles.GetFeedPage(cgi.Query);

            if (feedPage == null)
            {
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine(waffles.ErrorMessage);
                return;
            }
            RenderFeed(cgi, feedPage);
            Footer(cgi, feedPage);
        }


        public static void ProxyMedia(CgiWrapper cgi)
        {
            if(!cgi.HasQuery)
            {
                cgi.Redirect(CgiPaths.BasePath);
                return;
            }
            MediaProxy proxy = new MediaProxy();
            byte [] optimizedImage = proxy.ProxyMedia(cgi.Query);
            if(optimizedImage == null)
            {
                cgi.BadRequest("Couldn't fetch media");
                return;
            }
            cgi.Success("image/jpeg");
            cgi.Out.Write(optimizedImage);
        }

        private static void Footer(CgiWrapper cgi, AbstractPage page = null)
        {
            cgi.Writer.WriteLine();
            if (!string.IsNullOrEmpty(page.Meta.SiteName))
            {
                cgi.Writer.WriteLine($"All content © {DateTime.Now.Year} {page.Meta.SiteName}");
            }
            cgi.Writer.WriteLine("---");
            if (page != null)
            {
                cgi.Writer.WriteLine($"Size: {ReadableFileSize(page.Size)}. {page.Savings} smaller than original HTML: {ReadableFileSize(page.Meta.OriginalSize)} 🤮");
                cgi.Writer.WriteLine($"=> {page.Meta.OriginalUrl} Link to Source");
                cgi.Writer.WriteLine("---");
            }
            cgi.Writer.WriteLine("=> mailto:acidus@gemi.dev Made with 🧇 and ❤️ by Acidus");
        }

        private static void RenderLinks(CgiWrapper cgi, LinkPage homePage)
        {
            cgi.Writer.WriteLine($"## {homePage.Meta.Title}");
            if (homePage.Meta.FeaturedImage != null)
            {
                cgi.Writer.WriteLine($"=> {MediaRewriter.GetPath(homePage.Meta.FeaturedImage)} Featured Image");
            }
            if(homePage.Meta.Description.Length > 0)
            {
                cgi.Writer.WriteLine($">{homePage.Meta.Description}");
            }
            cgi.Writer.WriteLine($"=> {CgiPaths.ViewArticle(homePage.Meta.OriginalUrl)} Mode: Link View. This doesn't appear to be an article. Force Article View?");
            if (homePage.HasFeed)
            {
                cgi.Writer.WriteLine($"=> {CgiPaths.ViewFeed(homePage.FeedUrl)} RSS/Atom Feed detected. Click here for more accurate list of links");
            }
            cgi.Writer.WriteLine();

            int counter = 0;
            if (homePage.ContentLinks.Count > 0)
            {
                cgi.Writer.WriteLine($"### Content Links: {homePage.ContentLinks.Count}");
                
                foreach (var link in homePage.ContentLinks)
                {
                    counter++;
                    cgi.Writer.WriteLine($"=> {CgiPaths.ViewArticle(link.Url.AbsoluteUri)} {counter}. {link.Text}");
                }
            }
            if (homePage.NavigationLinks.Count > 0)
            {
                cgi.Writer.WriteLine($"### Navigation Links: {homePage.NavigationLinks.Count}");
                counter = 0;
                foreach (var link in homePage.NavigationLinks)
                {
                    counter++;
                    cgi.Writer.WriteLine($"=> {CgiPaths.ViewAuto(link.Url.AbsoluteUri)} {counter}. {link.Text}");
                }
            }
        }

        private static void RenderFeed(CgiWrapper cgi, FeedPage feedPage)
        {
            cgi.Writer.WriteLine($"## {feedPage.Meta.Title}");
            if (feedPage.Meta.FeaturedImage != null)
            {
                cgi.Writer.WriteLine($"=> {MediaRewriter.GetPath(feedPage.Meta.FeaturedImage)} Featured Image");
            }
            if (feedPage.Meta.Description.Length > 0)
            {
                cgi.Writer.WriteLine($">{feedPage.Meta.Description}");
            }
            if (feedPage.RootUrl.Length > 0)
            {
                cgi.Writer.WriteLine($"=> {CgiPaths.ViewLinks(feedPage.RootUrl)} View site homepage");
            }
            cgi.Writer.WriteLine();
            int counter = 0;
            cgi.Writer.WriteLine($"### Feed Links: {feedPage.Links.Count}");

            foreach (var link in feedPage.Links)
            {
                counter++;
                var published = link.HasPublished ? $"({link.TimeAgo})" : "";
                cgi.Writer.WriteLine($"=> {CgiPaths.ViewArticle(link.Url.AbsoluteUri)} {counter}. {link.Text} {published}");
            }            
        }

        private static void RenderArticle(CgiWrapper cgi, ContentPage articlePage)
        {
            if(articlePage.IsReadability)
            {
                RenderArticleSuccess(cgi, articlePage);
            } else
            {
                RenderArticleFailed(cgi, articlePage);
            }
        }

        private static void RenderArticleSuccess(CgiWrapper cgi, ContentPage articlePage)
        {
            cgi.Writer.WriteLine($"## {articlePage.Meta.Title}");
            if (articlePage.Published != null)
            {
                cgi.Writer.WriteLine($"Published: {articlePage.Published.Value.ToString("yyyy-MM-dd HH:mm")} GMT");
            }
            if (!string.IsNullOrEmpty(articlePage.Byline))
            {
                cgi.Writer.WriteLine($"Byline: {articlePage.Byline}");
            }
            cgi.Writer.WriteLine($"Length: {articlePage.WordCount} words (~{articlePage.TimeToRead.Minutes} minutes)");
            //if (articlePage.Images.Count > 0)
            //{
            //    cgi.Writer.WriteLine($"Article images: {articlePage.Images.Count}");
            //}
            //if (articlePage.Links.Count > 0)
            //{
            //    cgi.Writer.WriteLine($"Hyperlinks in Body: {articlePage.Links.Count}");
            //}
            if (articlePage.Meta.FeaturedImage != null)
            {
                cgi.Writer.WriteLine($"=> {MediaRewriter.GetPath(articlePage.Meta.FeaturedImage)} Featured Image");
            }

            foreach (var item in articlePage.Content)
            {
                cgi.Writer.Write(item.Content);
            }
        }

        private static void RenderArticleFailed(CgiWrapper cgi, ContentPage articlePage)
        {
            cgi.Writer.WriteLine($"## {articlePage.Meta.Title}");
            if (articlePage.Meta.FeaturedImage != null)
            {
                cgi.Writer.WriteLine($"=> {MediaRewriter.GetPath(articlePage.Meta.FeaturedImage)} Featured Image");
            }
            if(!string.IsNullOrEmpty(articlePage.Excerpt))
            {
                cgi.Writer.WriteLine("Excerpt:");
                cgi.Writer.WriteLine($">{articlePage.Excerpt}");
            }

            cgi.Writer.WriteLine("Based on meta data, we thought this was a article, But we were unable to extract one. This might be a page of primarily links, like a home page or category page. this full article could not be properly parsed.");
            cgi.Writer.WriteLine($"=> {CgiPaths.ViewLinks(articlePage.Meta.OriginalUrl)} Try in Link View");
        }

        static string ReadableFileSize(double size, int unit = 0)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            while (size >= 1024)
            {
                size /= 1024;
                ++unit;
            }

            return String.Format("{0:0.0#} {1}", size, units[unit]);
        }
    }
}
