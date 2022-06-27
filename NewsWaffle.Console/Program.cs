using NewsWaffle;

namespace NewsWaffle.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            CgiRouter router = new CgiRouter();
            router.OnRequest("", RouteHandler.Welcome);
            router.ProcessRequest();
        }
    }
}
