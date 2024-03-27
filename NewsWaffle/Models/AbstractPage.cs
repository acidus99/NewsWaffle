using System;
namespace NewsWaffle.Models;

public abstract class AbstractPage : IPageStats
{
    /// <summary>
    /// The amount of time in ms to convert the page from source
    /// </summary>
    public int ConvertTime { get; set; } = 0;

    /// <summary>
    /// The copyright for the site
    /// TODO: Make this nullable?
    /// </summary>
    public string Copyright => Meta.SiteName ?? "";

    /// <summary>
    /// The amount of time in ms to download the page from source
    /// </summary>
    public int DownloadTime { get; set; } = 0;

    /// <summary>
    /// The metadata for this page
    /// </summary>
    public PageMetaData Meta { get; private set; }

    /// <summary>
    /// The optimized size of the content
    /// </summary>
    public abstract int Size { get; }

    /// <summary>
    /// The original size of the content
    /// </summary>
    public int OriginalSize => Meta.OriginalSize;

    /// <summary>
    /// The URL for the original source
    /// </summary>
    public Uri SourceUrl => Meta.SourceUrl;

    public AbstractPage(PageMetaData metaData)
    {
        Meta = metaData;
    }
}
