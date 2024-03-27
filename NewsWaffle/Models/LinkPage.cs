using System;
using System.Collections.Generic;
using System.Linq;
using HtmlToGmi.Models;

namespace NewsWaffle.Models;

public class LinkPage : AbstractPage
{
    /// <summary>
    /// Collection of links to articles
    /// </summary>
    public List<Hyperlink> ArticleLinks { get; internal set; }

    /// <summary>
    /// Does this page have a feed?
    /// </summary>
    public bool HasFeed => (FeedUrl != null);

    /// <summary>
    /// Optional URL for any feed
    /// </summary>
    public Uri? FeedUrl { get; internal set; }

    /// <summary>
    /// Collection of links to other areas of the site
    /// </summary>
    public List<Hyperlink> NavigationLinks { get; internal set; }

    /// <summary>
    /// The optimized size. Estimated
    /// </summary>
    public override int Size => ComputeSize();

    public LinkPage(PageMetaData metaData)
        : base(metaData)
    {
        ArticleLinks = new List<Hyperlink>();
        NavigationLinks = new List<Hyperlink>();
    }

    private int ComputeSize()
    {
        int size = ArticleLinks.Sum(x => x.Size) + NavigationLinks.Sum(x => x.Size);
        size += (Meta.Description?.Length ?? 0);
        size += (Meta.Title?.Length ?? 0);
        size += 30; //overhead
        return size;
    }
}
