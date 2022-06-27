using System;
using System.Net;
using Gemini.Cgi;
using NewsWaffle.Models;

namespace NewsWaffle.Cgi
{
    public static class RouteHandler
    {
        public static void Article(CgiWrapper cgi)
        {
            if (!cgi.HasQuery)
            {
                cgi.Redirect("/cgi-bin/waffle.cgi");
                return;
            }
            cgi.Success();
            cgi.Writer.WriteLine("# 🧇 NewsWaffle");

            var waffles = new YummyWaffles();
            if (!waffles.GetPage(cgi.Query))
            {
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine(waffles.ErrorMessage);
                return;
            }
            if (!(waffles.Page is ArticlePage))
            {
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine("Url isn't an Article page");
                return;
            }

            var articlePage = (ArticlePage)waffles.Page;
            cgi.Writer.WriteLine($"## {articlePage.Title}");
            if (articlePage.FeaturedImage != null)
            {
                cgi.Writer.WriteLine($"=> {articlePage.FeaturedImage} Featured Image");
            }

            foreach (var item in articlePage.Content)
            {
                cgi.Writer.Write(item.Content);
            }
            Footer(cgi);
        }

        public static void Portal(CgiWrapper cgi)
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
            if(!(waffles.Page is HomePage))
            {
                cgi.Writer.WriteLine("Bummer dude! Error Wafflizing that page.");
                cgi.Writer.WriteLine("Url isn't a home page");
                return;
            }

            var homePage = (HomePage)waffles.Page;
                
            cgi.Writer.WriteLine($"Newsite: {homePage.Name}");
            cgi.Writer.WriteLine($"Links: {homePage.Links.Count}");
            foreach (var link in homePage.Links)
            {
                System.Console.WriteLine($"=> /cgi-bin/waffle.cgi/article?{WebUtility.UrlEncode(link.Url.AbsoluteUri)} {link.Number}. {link.Text}");
            }
            Footer(cgi);
        }

        public static void Welcome(CgiWrapper cgi)
        {
            cgi.Success();
            cgi.Writer.WriteLine("# 🧇 NewsWaffle");
            cgi.Writer.WriteLine("=> /cgi-bin/waffle.cgi/portal?https%3A%2F%2Fwww.wired.com%2F Wired.com");
            cgi.Writer.WriteLine("=> /cgi-bin/waffle.cgi/portal?https%3A%2F%2Fwww.irishtimes.com%2F Irish Times");
            Footer(cgi);
        }

        private static void Footer(CgiWrapper cgi)
        {
            cgi.Writer.WriteLine();
            cgi.Writer.WriteLine("---");
            cgi.Writer.WriteLine("=> mailto:acidus@gemi.dev Made with 🧇 and ❤️ by Acidus");
        }


    }
}

