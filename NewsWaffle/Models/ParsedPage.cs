using System;
using System.Linq;
using System.Collections.Generic;

namespace NewsWaffle.Models
{
    public class ParsedPage
    {
        public String Title { get; internal set; }

        public string SourceUrl { get; internal set; }

        public TimeSpan? TimeToRead { get; internal set; }

        public string FeaturedImage { get; internal set; }

        //content and images
        public List<ContentItem> Content = new List<ContentItem>();

        public List<MediaItem> Images = new List<MediaItem>();
    }
}
