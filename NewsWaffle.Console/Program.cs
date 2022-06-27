using NewsWaffle;
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

            //Step 1: Get HTML
            var fetcher = new HttpFetcher();
            var html = fetcher.GetHtml(args[0]);
            //step 2: Parse it to a type

            System.Console.WriteLine(html);


            //step 3: render it

        }
    }
}
