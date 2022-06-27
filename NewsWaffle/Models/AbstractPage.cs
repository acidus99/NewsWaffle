using System;
namespace NewsWaffle.Models
{
    public abstract class AbstractPage
    {
        public string FeaturedImage { get; internal set; }
        public int OriginalSize { get; internal set; }
        public String Title { get; internal set; }
        public string SourceUrl { get; internal set; }
    }
}

