using System;
using System.Collections.Generic;
using HtmlToGmi.Models;

namespace NewsWaffle.Models;

public class ArticlePage : AbstractPage
{
    /// <summary>
    /// Optional byline of the article
    /// </summary>
    public string? Byline { get; internal set; }

    /// <summary>
    /// The content of the page
    /// </summary>
    public string Content;

    /// <summary>
    /// Optional excerpt for the page. Only populated if we failed to extract a body
    /// </summary>
    public string? Excerpt { get; internal set; }

    /// <summary>
    /// Collection of images that are on the page
    /// </summary>
    public IReadOnlyCollection<ImageLink> Images;

    /// <summary>
    /// Were we able to extract a body
    /// TODO: Better name?
    /// </summary>
    public bool IsReadability { get; internal set; }

    /// <summary>
    /// Collection of Links found in the article
    /// </summary>
    public IReadOnlyCollection<Hyperlink> Links;

    /// <summary>
    /// Optional, estimated date the item was published
    /// </summary>
    public DateTime? Published { get; internal set; }

    /// <summary>
    /// Optimized size of the content
    /// TODO: Move this to the future RenderedPage object
    /// </summary>
    public override int Size
        => Content.Length;

    /// <summary>
    /// Estimated amount of time to read the content
    /// </summary>
    public TimeSpan TimeToRead { get; internal set; }

    /// <summary>
    /// Number of words in the content
    /// </summary>
    public int WordCount { get; internal set; } = 0;

    public ArticlePage(PageMetaData metaData)
        : base(metaData)
    {
        Content = "";
        Images = new List<ImageLink>();
        Links = new List<Hyperlink>();
    }
}
