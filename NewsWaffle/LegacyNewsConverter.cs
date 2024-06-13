using System;
using System.Diagnostics;
using NewsWaffle.Converters;
using NewsWaffle.Models;
using CacheComms;

namespace NewsWaffle;

/// <summary>
/// Fetches content over HTTP and converts it to different news formats
/// </summary>
public class LegacyNewsConverter
{
    public string ErrorMessage { get; internal set; } = "";

    //use an explicit cache
    HttpRequestor Requestor;
    bool UseCache;

    public LegacyNewsConverter(bool useCache =true)
    {
        Requestor = new HttpRequestor();
        UseCache = useCache;
    }

    /// <summary>
    /// Gets a page, auto-detect page type, and return it
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public AbstractPage GetPage(string urlString)
    {
        try
        {
            Uri originalUrl = LinkForge.Create(urlString);
            if (originalUrl == null)
            {
                return null;
            }
            //========= Step 1: Get HTML
            var response = GetContent(originalUrl);
            if (string.IsNullOrEmpty(response.Content))
            {
                return null;
            }

            if (IsFeed(response.Content))
            {
                var feed = FeedConverter.ParseFeed(response.RequestUrl, response.Content);
                feed.DownloadTime = (int)Requestor.DownloadTimeMs;
                return feed;
            }

            WebConverter htmlConverter = new WebConverter(response.RequestUrl, response.Content);

            //convert, autodetecting
            var page = htmlConverter.Convert();

            if (page == null)
            {
                ErrorMessage = $"Could not parse HTML from '{response.RequestUrl}'";
                return null;
            }
            page.DownloadTime = (int)Requestor.DownloadTimeMs;
            return page;

        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message + ex.StackTrace;
            return null;
        }
    }

    public FeedPage GetFeedPage(string urlString)
    {
        Uri originalUrl = LinkForge.Create(urlString);
        if (originalUrl == null)
        {
            return null;
        }
        var response = GetContent(originalUrl);
        if (string.IsNullOrEmpty(response.Content))
        {
            return null;
        }

        var page = FeedConverter.ParseFeed(response.RequestUrl, response.Content);
        if (page == null)
        {
            ErrorMessage = $"Could not parse RSS/Atom feed from '{response.RequestUrl}'";
            return null;
        }
        page.DownloadTime = (int)Requestor.DownloadTimeMs;
        return page;

    }

    /// <summary>
    /// Gets a page, force parsing it as a LinkPage
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public LinkPage GetLinkPage(string urlString)
    {
        try
        {
            Uri originalUrl = LinkForge.Create(urlString);
            if (originalUrl == null)
            {
                return null;
            }
            var response = GetContent(originalUrl);
            if (string.IsNullOrEmpty(response.Content))
            {
                return null;
            }

            WebConverter htmlConverter = new WebConverter(response.RequestUrl, response.Content);

            //convert, autodetecting
            var page = htmlConverter.ConvertToLinkPage();

            if (page == null)
            {
                ErrorMessage = $"Could not parse HTML from '{response.RequestUrl}'";
                return null;
            }
            page.DownloadTime = (int)Requestor.DownloadTimeMs;
            return page;

        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message + ex.StackTrace;
            return null;
        }
    }

    /// <summary>
    /// Gets a page, force parsing it as a LinkPage
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public ArticlePage GetContentPage(string urlString)
    {
        try
        {
            Uri originalUrl = LinkForge.Create(urlString);
            if (originalUrl == null)
            {
                return null;
            }
            var response = GetContent(originalUrl);
            if (string.IsNullOrEmpty(response.Content))
            {
                return null;
            }

            WebConverter htmlConverter = new WebConverter(response.RequestUrl, response.Content);

            //convert, autodetecting
            var page = htmlConverter.ConvertToContentPage();

            if (page == null)
            {
                ErrorMessage = $"Could not parse HTML from '{response.RequestUrl}'";
                return null;
            }
            page.DownloadTime = (int)Requestor.DownloadTimeMs;
            return page;

        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message + ex.StackTrace;
            return null;
        }
    }

    /// <summary>
    /// Gets a page, using Raw mode
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public RawPage GetRawPage(string urlString)
    {
        try
        {
            Uri originalUrl = LinkForge.Create(urlString);
            if (originalUrl == null)
            {
                return null;
            }
            //========= Step 1: Get HTML
            var response = GetContent(originalUrl);
            if (string.IsNullOrEmpty(response.Content))
            {
                return null;
            }

            var htmlConverter = new WebConverter(response.RequestUrl, response.Content);
            var page = htmlConverter.ConvertToRawPage();

            if (page == null)
            {
                ErrorMessage = $"Could not parse HTML from '{response.RequestUrl}'";
                return null;
            }
            page.DownloadTime = (int)Requestor.DownloadTimeMs;
            return page;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message + ex.StackTrace;
            return null;
        }
    }

    private (Uri RequestUrl, string? Content) GetContent(Uri originalUrl)
    {
        var result = Requestor.GetAsString(originalUrl, UseCache);

        if (!result)
        {
            ErrorMessage = Requestor.ErrorMessage;
            return (Requestor.RequestUri!, null);
        }

        return (Requestor.RequestUri!, Requestor.BodyText);
    }

    private bool IsFeed(string content)
    {
        var prefix = content.Substring(0, Math.Min(content.Length, 250));
        return prefix.Contains("<rss") || prefix.Contains("<feed");
    }
}