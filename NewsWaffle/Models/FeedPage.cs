using System;
using System.Collections.Generic;
using System.Linq;
using HtmlToGmi.NewsFeeds;

namespace NewsWaffle.Models;

public class FeedPage : AbstractPage
{
    /// <summary>
    /// Items in the feed
    /// </summary>
    public List<FeedItem> Items { get; } = new List<FeedItem>();

    /// <summary>
    /// The URL for the root of the site
    /// </summary>
    public Uri RootUrl { get; internal set; }

    /// <summary>
    /// The optimized size. Estimated
    /// </summary>
    public override int Size => ComputeSize();

    public FeedPage(PageMetaData metaData)
        : base(metaData)
    {
        RootUrl = GetRootUrl(SourceUrl);
    }

    private static Uri GetRootUrl(Uri url)
    {
        UriBuilder builder = new UriBuilder();
        builder.Scheme = url.Scheme;
        builder.Host = url.Host;
        builder.Port = url.Port;
        builder.Path = "/";
        return builder.Uri;
    }

    private int ComputeSize()
    {
        int size = Items.Sum(x => x.Title.Length + x.Url.AbsoluteUri.Length + 4);
        size += (Meta.Description?.Length ?? 0);
        size += (Meta.Title?.Length ?? 0);
        return size;
    }
}
