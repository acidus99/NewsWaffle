using System;
namespace NewsWaffle.Models;

public abstract class AbstractPage : IPageStats
{
    public AbstractPage(PageMetaData metaData)
    {
        Meta = metaData;
    }

    public PageMetaData Meta { get; private set; }
    public abstract int Size { get; }

    public int DownloadTime { get; set; } = 0;

    public int ConvertTime { get; set; } = 0;

    public int OriginalSize => Meta.OriginalSize;

    public string Copyright => Meta.SiteName ?? "";

    public Uri SourceUrl => Meta.SourceUrl ?? null;
}

