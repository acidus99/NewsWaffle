using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace NewsWaffle.Net
{
    public class HttpRequestor : IHttpRequestor
    {
        HttpClient Client;
        string Charset;
        HttpResponseMessage Response;

        public HttpRequestor()
        {
            Client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                CheckCertificateRevocationList = false,
                AutomaticDecompression = System.Net.DecompressionMethods.All,
            });

            Client.Timeout = TimeSpan.FromSeconds(20);
            Client.DefaultRequestHeaders.UserAgent.TryParseAdd("GeminiProxy/0.1 (gemini://gemi.dev/) gemini-proxy/0.1");
        }

        public string ErrorMessage { get; internal set; } = "";

        public byte[] BodyBytes { get; internal set; } = null;

        public string BodyText { get; internal set; } = null;

        public bool RequestAsBytes(Uri url)
        {
            if (!url.IsAbsoluteUri || !url.Scheme.StartsWith("http"))
            {
                ErrorMessage = "Only HTTP/HTTPS URLs are supported";
                return false;
            }

            Response = SendRequest(url);
            if (!Response.IsSuccessStatusCode)
            {
                ErrorMessage = $"Could not download content for URL. Statue code: '{Response.StatusCode}'";
                return false;
            }

            BodyBytes = ReadFully(Response.Content.ReadAsStream());
            return true;
        }

        public bool Request(Uri url)
        {
            var success = RequestAsBytes(url);
            if (!success)
            {
                return false;
            }

            var charset = GetCharset(Response.Content.Headers.ContentType);
            BodyText = Encoding.GetEncoding(charset).GetString(BodyBytes);
            if (string.IsNullOrEmpty(BodyText))
            {
                ErrorMessage = $"Received no content for '{url}'";
                return false;
            }
            
            return true;
        }



        private HttpResponseMessage SendRequest(Uri url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            return Client.Send(request, HttpCompletionOption.ResponseContentRead);
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static string GetCharset(MediaTypeHeaderValue contentType)
            => (contentType?.CharSet?.Length > 0) ?
                contentType.CharSet :
                "utf-8";
    }
}
