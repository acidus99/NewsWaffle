using System;
using System.Linq;
using System.Collections.Generic;

using HtmlToGmi.Models;

namespace NewsWaffle.Models
{
    public class LinkPage : AbstractPage
    {
        public LinkPage(PageMetaData metaData)
            : base(metaData)
        {
        }

        public bool HasFeed => !string.IsNullOrEmpty(FeedUrl);
        public string FeedUrl { get; internal set; }

        public List<Hyperlink> ContentLinks { get; internal set; }

        public List<Hyperlink> NavigationLinks { get; internal set; }

        public override int Size
            => ContentLinks.Sum(x => x.Size) +
                NavigationLinks.Sum(x => x.Size) +
                Meta.Description.Length +
                Meta.Title.Length + 30;
    }
}

