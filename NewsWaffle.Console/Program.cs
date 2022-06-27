using NewsWaffle.Converter;
using NewsWaffle.Models;
using NewsWaffle.Net;

namespace NewsWaffle.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                System.Console.WriteLine("Missing URL");
            }

            var url = args[0];

            //========= Step 1: Get HTML
            var fetcher = new HttpFetcher();
            var html = fetcher.GetHtml(url);


            //========= Step 2: Parse it to a type
            var converter = new HtmlConverter();
            var page = converter.ParseHtmlPage(url, html);

            //========= Step 3: Render it
            if (page == null)
            {
                System.Console.WriteLine($"Could not parse HTML from '{url}'");
                return;
            }
            SaveHtml(page);

            if (page is HomePage)
            {
                RenderHomePage((HomePage)page);
            }
            else if (page is ArticlePage)
            {
                RenderArticle((ArticlePage)page);
            }
        }

        private static void RenderHomePage(HomePage homePage)
        {
            System.Console.WriteLine("Home Page");
            System.Console.WriteLine($"Title: {homePage.Name}");
            foreach (var link in homePage.Links)
            {
                System.Console.WriteLine($"'{link.Text}' => '{link.Url}'");
            }
        }

        private static void SaveHtml(AbstractPage page)
        {
            var html = (page is ArticlePage) ? ((ArticlePage)page).SimplifiedHtml : "";
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/tmp/out.html", html);
        }

        private static void RenderArticle(ArticlePage articlePage)
        {
            System.Console.WriteLine("Article Page");
            System.Console.WriteLine($"Title: {articlePage.Title}");
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
