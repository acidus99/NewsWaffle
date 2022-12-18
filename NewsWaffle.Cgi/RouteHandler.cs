using System.Net;
using Gemini.Cgi;

using NewsWaffle.Models;
using NewsWaffle.Cgi.Views;
using NewsWaffle.Cgi.Media;

using NewsWaffle.Aggregators;
using NewsWaffle.Aggregators.Models;


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

            var waffles = new YummyWaffles();
            var page = waffles.GetPage(cgi.Query);
            if (page == null)
            {
                cgi.Writer.WriteLine("# 🧇 NewsWaffle");
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine(waffles.ErrorMessage);
                return;
            }

            if (page is LinkPage)
            {
                RenderLinks(cgi, (LinkPage)page);
            }
            else if (page is ContentPage)
            {
                var articleView = new ArticleView(cgi.Writer);
                articleView.RenderArticle((ContentPage)page);
            }
            else
            {
                RenderFeed(cgi, (FeedPage)page);
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
            var waffles = new YummyWaffles();
            var page = waffles.GetContentPage(cgi.Query);

            if (page == null)
            {
                cgi.Writer.WriteLine($"# 🧇 NewsWaffle");
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine(waffles.ErrorMessage);
                return;
            }
            var articleView = new ArticleView(cgi.Writer);
            articleView.RenderArticle((ContentPage)page);

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
            var waffles = new YummyWaffles();
            var page = waffles.GetLinkPage(cgi.Query);

            if (page == null)
            {
                cgi.Writer.WriteLine($"# 🧇 NewsWaffle");
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
            cgi.Writer.WriteLine($"# 🧇 NewsWaffle");

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

        public static void Raw(CgiWrapper cgi)
        {
            if (!cgi.HasQuery)
            {
                cgi.Redirect(CgiPaths.BasePath);
                return;
            }
            cgi.Success();
            cgi.Writer.WriteLine($"# 🧇 NewsWaffle");

            var waffles = new YummyWaffles();
            var page = waffles.GetRawPage(cgi.Query);
            if (page == null)
            {
                cgi.Writer.WriteLine("# 🧇 NewsWaffle");
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine(waffles.ErrorMessage);
                return;
            }

            RenderRaw(cgi, page);
            Footer(cgi, page);
        }

        public static void CurrentNews(CgiWrapper cgi)
        {
            var yahooNews = new YahooNews();

            var sectionName = cgi.HasQuery ? cgi.SantiziedQuery : yahooNews.DefaultSection;

            if(!yahooNews.IsValidSection(sectionName))
            {
                cgi.Redirect(CgiPaths.ViewCurrentNewsSection());
                return;
            }

            cgi.Success();
            cgi.Writer.WriteLine($"# 🧇 NewsWaffle - Current News");

            var newsSection = AggregatorFetcher.GetSection(sectionName, yahooNews);

            if (newsSection == null)
            {
                cgi.Writer.WriteLine("Bummer dude! Error fetching current news");
                return;
            }
            RenderCurrentNewsSection(cgi, newsSection);
            Footer(cgi, newsSection);
        }

        public static void SwitchNewsSection(CgiWrapper cgi)
        {
            var yahooNews = new YahooNews();
            cgi.Success();
            cgi.Writer.WriteLine($"# 🧇 NewsWaffle - Current News");
            cgi.Writer.WriteLine("Choose a section:");
            foreach(var section in yahooNews.AvailableSections)
            {
                cgi.Writer.WriteLine($"=> {CgiPaths.ViewCurrentNewsSection(section)} {section}");
            }
            Footer(cgi);
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

        private static void Footer(CgiWrapper cgi, IPageStats page = null)
        {
            cgi.Writer.WriteLine();
            if (!string.IsNullOrEmpty(page?.Copyright ?? null))
            {
                cgi.Writer.WriteLine($"All content © {DateTime.Now.Year} {page.Copyright}");
            }
            cgi.Writer.WriteLine("---");
            if (page != null)
            {
                cgi.Writer.WriteLine($"Size: {RenderUtils.ReadableFileSize(page.Size)}. {RenderUtils.Savings(page.Size, page.OriginalSize)} smaller than original: {RenderUtils.ReadableFileSize(page.OriginalSize)} 🤮");
                cgi.Writer.WriteLine($"Fetched: {page.DownloadTime} ms. Converted: {page.ConvertTime} ms 🐇");
                cgi.Writer.WriteLine($"=> {page.SourceUrl} Link to Source");
                cgi.Writer.WriteLine("---");
            }
            cgi.Writer.WriteLine("=> mailto:acidus@gemi.dev Made with 🧇 and ❤️ by Acidus");
        }

        private static void RenderLinks(CgiWrapper cgi, LinkPage homePage)
        {
            cgi.Writer.WriteLine($"# 🧇 NewsWaffle: {homePage.Meta.SiteName}");

            cgi.Writer.WriteLine($"## {homePage.Meta.Title}");
            if (homePage.Meta.FeaturedImage != null)
            {
                cgi.Writer.WriteLine($"=> {MediaRewriter.GetPath(homePage.Meta.FeaturedImage)} Featured Image");
            }
            if(homePage.Meta.Description.Length > 0)
            {
                cgi.Writer.WriteLine($">{homePage.Meta.Description}");
            }
            cgi.Writer.WriteLine($"=> {CgiPaths.ViewArticle(homePage.Meta.SourceUrl)} Mode: Link View. This doesn't appear to be an article. Force Article View?");
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
                cgi.Writer.WriteLine($"=> {CgiPaths.ViewLinks(feedPage.RootUrl)} Mode: RSS View. Try Link View?");
            }
            cgi.Writer.WriteLine();
            cgi.Writer.WriteLine($"### Feed Links: {feedPage.Links.Count}");
            if (feedPage.Links.Count > 0)
            {
                int counter = 0;
                foreach (var link in feedPage.Links)
                {
                    counter++;
                    var published = link.HasPublished ? $"({link.TimeAgo})" : "";
                    cgi.Writer.WriteLine($"=> {CgiPaths.ViewArticle(link.Url.AbsoluteUri)} {counter}. {link.Text} {published}");
                }
            }
            else
            {
                cgi.Writer.WriteLine("This feed doesn't actually have any items in it. Unfortunately some sites have broken feeds without content."); ;
            }
        }

        private static void RenderRaw(CgiWrapper cgi, RawPage rawPage)
        {
            cgi.Writer.WriteLine($"## {rawPage.Meta.Title}");
            if (rawPage.Meta.FeaturedImage != null)
            {
                cgi.Writer.WriteLine($"=> {MediaRewriter.GetPath(rawPage.Meta.FeaturedImage)} Featured Image");
            }
            cgi.Writer.WriteLine($"=> {CgiPaths.ViewArticle(rawPage.Meta.SourceUrl)} Mode: Raw View. Try in Article View?");
            cgi.Writer.WriteLine();
            foreach (var item in rawPage.Content)
            {
                cgi.Writer.Write(item.Content);
            }
        }

        private static void RenderCurrentNewsSection(CgiWrapper cgi, NewsSection section)
        {
            cgi.Writer.WriteLine($"=> {CgiPaths.SwitchNewsSection} Current Section: {section.SectionName}. Change?");
            cgi.Writer.WriteLine();
            if (section.Stories.Count > 0)
            {
                int counter = 0;
                foreach (var story in section.Stories.OrderByDescending(x=>x.Updated))
                {
                    counter++;
                    cgi.Writer.WriteLine($"=> {CgiPaths.ViewArticle(story.Url)} {counter}. {story.Title} ({story.Source}, {story.TimeAgo})");
                }
            }
            else
            {
                cgi.Writer.WriteLine("This sections doesn't have any news stories"); ;
            }
        }
    }
}