using System;
using System.Linq;
using System.Collections.Generic;


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

        public List<HyperLink> ContentLinks { get; internal set; }

        public List<HyperLink> NavigationLinks { get; internal set; }

        public override int Size
            => ContentLinks.Sum(x => x.Size) +
                NavigationLinks.Sum(x => x.Size) +
                Meta.Description.Length +
                Meta.Title.Length + 30;
    }
}

