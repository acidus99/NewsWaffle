using System;
using System.Linq;
using System.Collections.Generic;

using HtmlToGmi.NewsFeeds;

namespace NewsWaffle.Models;

public class FeedPage : AbstractPage
{
    public Uri RootUrl { get; internal set; }
    public List<FeedItem> Items { get; } = new List<FeedItem>();

    public FeedPage(PageMetaData metaData)
        : base(metaData)
    {
    }

    public override int Size =>
        Items.Sum(x => x.Title.Length + x.Url.AbsoluteUri.Length + 4) +
            Meta.Description.Length +
            Meta.Title.Length + 30;
}

