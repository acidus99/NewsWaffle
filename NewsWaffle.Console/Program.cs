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
                    if (!waffles.GetPage(url))
                    {
                        System.Console.WriteLine($"Error: '{waffles.ErrorMessage}'");
                        return;
                    }
                    var page = waffles.Page;
                    //========= Step 3: Render it

                    if (page is HomePage)
                    {
                        RenderHomePage((HomePage)page);
                    }
                    else if (page is ArticlePage)
                    {
                        RenderArticle((ArticlePage)page);
                    }
                }
                System.Console.WriteLine("Entry URL");
                url = System.Console.ReadLine();

            } while (url != "quit");
        }

        private static void RenderHomePage(HomePage homePage)
        {
            System.Console.WriteLine($"## Title: {homePage.Title}");
            if (homePage.FeaturedImage != null)
            {
                System.Console.WriteLine($"=> {homePage.FeaturedImage} Featured Image");
            }
            if (homePage.Description.Length > 0)
            {
                System.Console.WriteLine($">{homePage.Description}");
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

        private static void RenderArticle(ArticlePage articlePage)
        {
            System.Console.WriteLine($"## {articlePage.Title}");
            if (articlePage.FeaturedImage != null)
            {
                System.Console.WriteLine($"=> {articlePage.FeaturedImage} Featured Image");
            }

            foreach (var item in articlePage.Content)
            {
                System.Console.Write(item.Content);
            }
        }
    }
}
