using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace NewsWaffle.Net;

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
        EmulateBrowser(Client);
    }

    public string ErrorMessage { get; internal set; } = "";

    public byte[] BodyBytes { get; internal set; } = null;

    public string BodyText { get; internal set; } = null;

    public bool RequestAsBytes(Uri url)
    {
        if (!IsValidUrl(url))
        {
            ErrorMessage = "Only HTTP/HTTPS URLs are supported";
            return false;
        }
        try
        {
            Response = SendRequest(url);
        }
        catch (System.Threading.Tasks.TaskCanceledException)
        {
            ErrorMessage = "Could not download content for URL. Connection Timeout.";
            return false;
        }

        if (!Response.IsSuccessStatusCode)
        {
            ErrorMessage = $"Could not download content for URL. Status code: '{Response.StatusCode}'";
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

    private void EmulateBrowser(HttpClient client)
    {
        // Use HTTP headers sent by MacOS Safari Version 17.3.1 (19617.2.4.11.12)
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.3.1 Safari/605.1.15");
        client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
        client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
        client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
        client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
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

    public static bool IsValidUrl(Uri url)
        => url.IsAbsoluteUri && url.Scheme.StartsWith("http");

}
