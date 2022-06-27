using System;
using System.Net;
namespace NewsWaffle.Net
{
    public class HttpFetcher
    {
        WebClient client;

        public HttpFetcher()
        {
            client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "GeminiProxy/0.1 (gemini://gemi.dev/; acidus@gemi.dev) gemini-proxy/0.1");
        }

        public string GetHtml(string url)
            => client.DownloadString(url);
    }
}