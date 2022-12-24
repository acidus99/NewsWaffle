using System;
using System.Linq;
using System.Collections.Generic;

using HtmlToGmi.Models;

namespace NewsWaffle.Models
{
    public class ContentPage : AbstractPage
    {
        public string Byline { get; internal set; }
        public DateTime? Published { get; internal set; }

        //content and images
        public string Content = "";
        public List<Image> Images = new List<Image>();
        public List<Hyperlink> Links = new List<Hyperlink>();

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
            => Content.Length;
    }
}
