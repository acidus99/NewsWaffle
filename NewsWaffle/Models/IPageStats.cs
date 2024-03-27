using System;

namespace NewsWaffle.Models;

/// <summary>
/// Statistics about the page
/// </summary>
public interface IPageStats
{
    /// <summary>
    /// The amount of time in ms to convert the page from source
    /// </summary>
    int ConvertTime { get; }

    /// <summary>
    /// The copyright for the site
    /// TODO: Make this nullable?
    /// </summary>
    string Copyright { get; }

    /// <summary>
    /// The amount of time in ms to download the page from source
    /// </summary>
    int DownloadTime { get; }

    /// <summary>
    /// The original size of the content
    /// </summary>
    int OriginalSize { get; }

    /// <summary>
    /// The optimized size of the content
    /// </summary>
    int Size { get; }

    /// <summary>
    /// The URL for the original source
    /// </summary>
    Uri SourceUrl { get; }
}
