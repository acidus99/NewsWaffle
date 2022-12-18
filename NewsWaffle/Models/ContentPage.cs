using System;
using System.Linq;
using System.Collections.Generic;

namespace NewsWaffle.Models
{
    public class ContentPage : AbstractPage
    {
        public string Byline { get; internal set; }
        public DateTime? Published { get; internal set; }

        //content and images
        public List<ContentItem> Content = new List<ContentItem>();
        public List<MediaItem> Images = new List<MediaItem>();
        public LinkCollection Links = new LinkCollection();

        public bool IsReadability { get; internal set; }

        //we only populate excerpt if we couldn't extract body a failure
        public string Excerpt { get; internal set; }

        public ContentPage(PageMetaData metaData)
            : base(metaData)
        {
        }

        public TimeSpan TimeToRead { get; internal set; }

        public int WordCount { get; internal set; } = 0;

        public override int Size
            => Content.Sum(x => x.Content.Length);
    }
}
