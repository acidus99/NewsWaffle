using System;
namespace NewsWaffle.Models
{
    public abstract class AbstractPage
    {
        public string FeaturedImage { get; internal set; }
        public int OriginalSize { get; internal set; }
        public String Title { get; internal set; }
        public string SourceUrl { get; internal set; }

        public abstract int Size { get; }

        public string Savings
            => string.Format("{0:0.00}%", (1.0d - (Convert.ToDouble(Size) / Convert.ToDouble(OriginalSize))) * 100.0d);
    }
}

