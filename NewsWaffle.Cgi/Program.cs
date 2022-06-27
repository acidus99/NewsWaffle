using Gemini.Cgi;

namespace NewsWaffle.Cgi
{
    class Program
    {
        static void Main(string[] args)
        {
            CgiRouter router = new CgiRouter();
            router.OnRequest("/article", RouteHandler.Article);
            router.OnRequest("/portal", RouteHandler.Portal);
            router.OnRequest("", RouteHandler.Welcome);
            router.ProcessRequest();
        }
    }
}
