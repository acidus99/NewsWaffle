﻿using System.Net;
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
                RenderHome(cgi, (LinkPage)page);
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

        public static void ProxyMedia(CgiWrapper cgi)
        {
            if(!cgi.HasQuery)
            {
                cgi.Redirect("/cgi-bin/waffle.cgi");
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

        private static void RenderHome(CgiWrapper cgi, LinkPage homePage)
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
            cgi.Writer.WriteLine();
            int counter = 0;
            if (homePage.ContentLinks.Count > 0)
            {
                cgi.Writer.WriteLine($"### Content Links: {homePage.ContentLinks.Count}");
                
                foreach (var link in homePage.ContentLinks)
                {
                    counter++;
                    cgi.Writer.WriteLine($"=> /cgi-bin/waffle.cgi/view?{WebUtility.UrlEncode(link.Url.AbsoluteUri)} {counter}. {link.Text}");
                }
            }
            if (homePage.NavigationLinks.Count > 0)
            {
                cgi.Writer.WriteLine($"### Navigation Links: {homePage.NavigationLinks.Count}");
                counter = 0;
                foreach (var link in homePage.NavigationLinks)
                {
                    counter++;
                    cgi.Writer.WriteLine($"=> /cgi-bin/waffle.cgi/view?{WebUtility.UrlEncode(link.Url.AbsoluteUri)} {counter}. {link.Text}");
                }
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
            if (articlePage.Images.Count > 0)
            {
                cgi.Writer.WriteLine($"Article images: {articlePage.Images.Count}");
            }
            if (articlePage.Links.Count > 0)
            {
                cgi.Writer.WriteLine($"Hyperlinks in Body: {articlePage.Links.Count}");
            }
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

            cgi.Writer.WriteLine("Unfortunately this full article could not be properly parsed.");
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
