using NewsWaffle.Converter;
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

            int x = 4;
            //step 3: render it

        }
    }
}
