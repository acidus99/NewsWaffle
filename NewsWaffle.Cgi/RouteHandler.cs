using System;
using System.Net;
using Gemini.Cgi;
using NewsWaffle.Models;

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
            if(!waffles.GetPage(cgi.Query))
            {
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine(waffles.ErrorMessage);
                return;
            }

            if (waffles.Page is HomePage)
            {
                RenderHome(cgi, (HomePage)waffles.Page);
            }
            else
            {
                RenderArticle(cgi, (ArticlePage)waffles.Page);
            }

            Footer(cgi);
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
            if (!waffles.GetPage(cgi.Query, true))
            {
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine(waffles.ErrorMessage);
                return;
            }
            RenderArticle(cgi, (ArticlePage)waffles.Page);
            Footer(cgi);
        }

        public static void Welcome(CgiWrapper cgi)
        {
            cgi.Success();
            cgi.Writer.WriteLine("# 🧇 NewsWaffle");

            cgi.Writer.WriteLine("=> /cgi-bin/waffle.cgi/view View news site");
            cgi.Writer.WriteLine("## Examples:");
            cgi.Writer.WriteLine("=> /cgi-bin/waffle.cgi/view?https%3A%2F%2Fwww.wired.com%2F Wired");
            cgi.Writer.WriteLine("=> /cgi-bin/waffle.cgi/view?https%3A%2F%2Fwww.ajc.com%2F Atlanta Journal-Constitution");
            cgi.Writer.WriteLine("=> /cgi-bin/waffle.cgi/view?https%3A%2F%2Fwww.sixcolors.com%2F Six Colors");
            Footer(cgi);
        }

        private static void Footer(CgiWrapper cgi)
        {
            cgi.Writer.WriteLine();
            cgi.Writer.WriteLine("---");
            cgi.Writer.WriteLine("=> mailto:acidus@gemi.dev Made with 🧇 and ❤️ by Acidus");
        }

        private static void RenderHome(CgiWrapper cgi, HomePage homePage)
        {
            cgi.Writer.WriteLine($"## {homePage.Title}");
            if (homePage.FeaturedImage != null)
            {
                cgi.Writer.WriteLine($"=> {homePage.FeaturedImage} Featured Image");
            }
            if(homePage.Description.Length > 0)
            {
                cgi.Writer.WriteLine($">{homePage.Description}");
            }
            cgi.Writer.WriteLine();
            cgi.Writer.WriteLine($"### Content Links: {homePage.ContentLinks.Count}");
            int counter = 0;
            foreach (var link in homePage.ContentLinks)
            {
                counter++;
                cgi.Writer.WriteLine($"=> /cgi-bin/waffle.cgi/view?{WebUtility.UrlEncode(link.Url.AbsoluteUri)} {counter}. {link.Text}");
            }
            cgi.Writer.WriteLine($"### Navigation Links: {homePage.NavigationLinks.Count}");
            counter = 0;
            foreach (var link in homePage.NavigationLinks)
            {
                counter++;
                cgi.Writer.WriteLine($"=> /cgi-bin/waffle.cgi/view?{WebUtility.UrlEncode(link.Url.AbsoluteUri)} {counter}. {link.Text}");
            }
        }

        private static void RenderArticle(CgiWrapper cgi, ArticlePage articlePage)
        {
            cgi.Writer.WriteLine($"## {articlePage.Title}");
            cgi.Writer.WriteLine($"Length: {articlePage.WordCount} words (~{articlePage.TimeToRead.Minutes} minutes)");
            if(articlePage.Images.Count > 0)
            {
                cgi.Writer.WriteLine($"Article images: {articlePage.Images.Count}");
            }
            if (articlePage.FeaturedImage != null)
            {
                cgi.Writer.WriteLine($"=> {articlePage.FeaturedImage} Featured Image");
            }

            foreach (var item in articlePage.Content)
            {
                cgi.Writer.Write(item.Content);
            }
            cgi.Writer.WriteLine();
            cgi.Writer.WriteLine($"🤏 Size {articlePage.Size}. Original HTML was {articlePage.OriginalSize} 🤮");
        }
    }
}

