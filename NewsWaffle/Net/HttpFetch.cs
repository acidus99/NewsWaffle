using System;
using System.Net;

using NewsWaffle.Cache;

namespace NewsWaffle.Net
{
    public class HttpFetcher
    {
        WebClient Client;
        DiskCache Cache;


        public HttpFetcher()
        {
            Client = new WebClient();
            Client.Headers.Add(HttpRequestHeader.UserAgent, "GeminiProxy/0.1 (gemini://gemi.dev/; acidus@gemi.dev) gemini-proxy/0.1");
            Cache = new DiskCache(TimeSpan.FromHours(1));
        }

        public string GetHtml(string url)
        {
            //first check the cache
            var contents = Cache.GetAsString(url);
            if (contents != null)
            {
                return contents;
            }
            //fetch it
            contents = Client.DownloadString(url);
            //cache it
            Cache.Set(url, contents);
            return contents;
        }
    }
}