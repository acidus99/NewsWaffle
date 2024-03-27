using System;
using System.Text;
using NewsWaffle.Cache;

namespace NewsWaffle.Net;

/// <summary>
/// Adds a caching layer to our HTTP Request
/// </summary>
public class CachingRequestor : IHttpRequestor
{
    DiskCache Cache;
    HttpRequestor Requestor;

    public byte[] BodyBytes { get; internal set; } = null!;

    public string BodyText { get; internal set; } = null!;

    public string ErrorMessage { get; internal set; } = "";

    public CachingRequestor(TimeSpan cacheDuration)
    {
        Cache = new DiskCache(cacheDuration);
        Requestor = new HttpRequestor();
    }

    public bool Request(Uri url)
    {
        if (!IsValidUrl(url))
        {
            return false;
        }

        //try and get it from the cache
        var content = GetFromCache(url);

        if (content != null)
        {
            BodyBytes = content;
            //strings are always UTF-8 in the cache
            BodyText = Encoding.UTF8.GetString(BodyBytes);
            return true;
        }

        //nope, go get it from the network
        var result = Requestor.Request(url);
        if (!result)
        {
            //error getting it from the network, pass the error message through
            ErrorMessage = Requestor.ErrorMessage;
            return false;
        }

        BodyBytes = Requestor.BodyBytes;
        BodyText = Requestor.BodyText;

        //ok, we are good, store this in the cache
        PutInCache(url, BodyBytes);
        return true;
    }

    public bool RequestAsBytes(Uri url)
    {
        if (!IsValidUrl(url))
        {
            return false;
        }

        //try and get it from the cache
        var content = GetFromCache(url);

        if (content != null)
        {
            BodyBytes = content;
            return true;
        }

        //nope, go get it from the network
        var result = Requestor.Request(url);
        if (!result)
        {
            //error getting it from the network, pass the error message through
            ErrorMessage = Requestor.ErrorMessage;
            return false;
        }

        BodyBytes = Requestor.BodyBytes;

        //ok, we are good, store this in the cache
        PutInCache(url, BodyBytes);
        return true;
    }

    private bool IsValidUrl(Uri url)
    {
        if (!HttpRequestor.IsValidUrl(url))
        {
            ErrorMessage = "Only HTTP/HTTPS URLs are supported";
            return false;
        }
        return true;
    }

    private byte[]? GetFromCache(Uri url)
      => Cache.GetAsBytes(GetCacheKey(url));

    private void PutInCache(Uri url, byte[] data)
        => Cache.Set(GetCacheKey(url), data);

    private static string GetCacheKey(Uri url)
        => url.AbsoluteUri;

}

