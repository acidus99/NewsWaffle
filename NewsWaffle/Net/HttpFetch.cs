using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

using NewsWaffle.Cache;

namespace NewsWaffle.Net
{
    public class HttpFetcher
    {
        DiskCache Cache;
        HttpClient Client;

        public HttpFetcher()
        {
            Client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                //AutomaticDecompression = System.Net.DecompressionMethods.All
            });
            Client.DefaultRequestHeaders.UserAgent.TryParseAdd("GeminiProxy/0.1 (gemini://gemi.dev/) gemini-proxy/0.1");
            Cache = new DiskCache(TimeSpan.FromHours(1));
        }

        public string GetAsString(string url)
        {
            try
            {
                //first check the cache
                var contents = Cache.GetAsString(url);
                if (contents != null)
                {
                    return contents;
                }
                //fetch it
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                contents = ReadAsString(url);
                stopwatch.Stop();
                Console.WriteLine("MS : " + Convert.ToInt32(stopwatch.ElapsedMilliseconds));
                //cache it
                Cache.Set(url, contents);
                return contents;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public byte [] GetAsBytes(string url)
        {
            try
            {
                //first check the cache
                var contents = Cache.GetAsBytes(url);
                if (contents != null)
                {
                    return contents;
                }
                //fetch it
                contents = ReadAsButes(url);
                //cache it
                Cache.Set(url, contents);
                return contents;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private HttpResponseMessage MakeRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            return Client.Send(request);
        }

        private string ReadAsString(string url)
        {
            var resp = MakeRequest(url);

            var reader = new StreamReader(resp.Content.ReadAsStream());
            return reader.ReadToEnd();
        }

        private byte[] ReadAsButes(string url)
        {
            var resp = MakeRequest(url);

            MemoryStream ms = new MemoryStream();
            return resp.Content.ReadAsByteArrayAsync().Result;

        }

    }
}