using NewsWaffle;
using NewsWaffle.Converter;
using NewsWaffle.Models;
using NewsWaffle.Net;

namespace NewsWaffle.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "";

            if(args.Length > 0)
            {
                url = args[0];
            }

            do
            {
                if (url.StartsWith("http"))
                {
                    var waffles = new YummyWaffles();
                    var page = waffles.GetPage(url);
                    if (page == null)
                    {
                        System.Console.WriteLine($"Error: '{waffles.ErrorMessage}'");
                        return;
                    }
                    if (page is LinkPage)
                    {
                        RenderHomePage((LinkPage)page);
                    }
                    else if (page is ContentPage)
                    {
                        RenderArticle((ContentPage)page);
                    }
                }
                System.Console.WriteLine("Entry URL");
                url = System.Console.ReadLine().Trim();

            } while (url != "quit");
        }

        private static void RenderHomePage(LinkPage homePage)
        {
            System.Console.WriteLine($"## Title: {homePage.Meta.Title}");
            if (homePage.Meta.FeaturedImage != null)
            {
                System.Console.WriteLine($"=> {homePage.Meta.FeaturedImage} Featured Image");
            }
            if (homePage.Meta.Description.Length > 0)
            {
                System.Console.WriteLine($">{homePage.Meta.Description}");
            }
            System.Console.WriteLine($"Content Links: {homePage.ContentLinks.Count}");
            foreach (var link in homePage.ContentLinks)
            {
                System.Console.WriteLine($"'{link.Text}' => '{link.Url}'");
            }

            System.Console.WriteLine($"Navigation Links: {homePage.NavigationLinks.Count}");
            foreach (var link in homePage.NavigationLinks)
            {
                System.Console.WriteLine($"'{link.Text}' => '{link.Url}'");
            }
        }

        private static void RenderArticle(ContentPage articlePage)
        {
            System.Console.WriteLine($"## {articlePage.Meta.Title}");
            if (articlePage.Meta.FeaturedImage != null)
            {
                System.Console.WriteLine($"=> {articlePage.Meta.FeaturedImage} Featured Image");
            }

            foreach (var item in articlePage.Content)
            {
                System.Console.Write(item.Content);
            }
        }
    }
}
