using System;

namespace NewsWaffle.Models;

/// <summary>
/// Represents the media data for an article, link page, or feed
/// </summary>
public class PageMetaData
{
    /// <summary>
    /// Description of the page
    /// </summary>
    public string? Description { get; internal set; }

    /// <summary>
    /// Optional feature image
    /// </summary>
    public Uri? FeaturedImage { get; internal set; }

    /// <summary>
    /// The size of the original page
    /// </summary>
    public int OriginalSize { get; init; }

    /// <summary>
    /// The URL of the original page
    /// </summary>
    public required Uri SourceUrl { get; init; }

    /// <summary>
    /// The name of the site
    /// </summary>
    public required string SiteName { get; init; }

    /// <summary>
    /// The title of the original page
    /// </summary>
    public string? Title { get; internal set; }

    /// <summary>
    /// Estimated type of page
    /// </summary>
    public PageType ProbablyType { get; internal set; }
}
