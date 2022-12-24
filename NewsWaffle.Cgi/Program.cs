using Gemini.Cgi;

namespace NewsWaffle.Cgi
{
    class Program
    {
        static void Main(string[] args)
        {

            LinkRewriter.ImageProxy = CgiPaths.MediaProxyEndpoint;
            LinkRewriter.LinkProxy = CgiPaths.AutoViewEndpoint;

            CgiRouter router = new CgiRouter();
            router.OnRequest("/article", RouteHandler.Article);
            router.OnRequest("/current", RouteHandler.CurrentNews);
            router.OnRequest("/feed", RouteHandler.Feed);
            router.OnRequest("/links", RouteHandler.Links);
            router.OnRequest("/raw", RouteHandler.Raw);
            router.OnRequest("/switch", RouteHandler.SwitchNewsSection);
            router.OnRequest("/view", RouteHandler.View);
            router.OnRequest("/media.jpg", RouteHandler.ProxyMedia);
            router.SetStaticRoot("static/");
            router.ProcessRequest();
        }
    }
}
