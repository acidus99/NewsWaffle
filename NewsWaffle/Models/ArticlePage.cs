using System;
using System.Linq;
using System.Collections.Generic;

namespace NewsWaffle.Models
{
    public class ArticlePage : AbstractPage
    {
        public string Byline { get; internal set; }
        public DateTime? Published { get; internal set; }

        //content and images
        public List<ContentItem> Content = new List<ContentItem>();
        public List<MediaItem> Images = new List<MediaItem>();

        //For debugging
        public string SimplifiedHtml { get; set; }

        public TimeSpan TimeToRead { get; internal set; }

        public int WordCount { get; internal set; }

        public override int Size
            => Content.Sum(x => x.Content.Length);


    }
}
