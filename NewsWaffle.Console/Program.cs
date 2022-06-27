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

            //Step 1: Get HTML
            var fetcher = new HttpFetcher();
            var html = fetcher.GetHtml(url);


            //step 2: Parse it to a type
            HtmlConverter converter = new HtmlConverter();
            var page = converter.ParseHtmlPage(url, html);

            
            //step 3: render it
            if(page is HomePage)
            {
                var homePage = ((HomePage)page);

                System.Console.WriteLine("Homepage");
                System.Console.WriteLine($"Title: {homePage.Name}");
                foreach(var link in homePage.Links)
                {
                    System.Console.WriteLine($"'{link.Text}' => '{link.Url}'");
                }
            }

            int x = 4;

        }
    }
}
