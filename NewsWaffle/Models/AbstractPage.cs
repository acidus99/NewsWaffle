using System;
namespace NewsWaffle.Models
{
    public abstract class AbstractPage
    {

        public AbstractPage(PageMetaData metaData)
        {
            Meta = metaData;
        }

        public PageMetaData Meta { get; private set; }
        public abstract int Size { get; }

        public string Savings
            => string.Format("{0:0.00}%", (1.0d - (Convert.ToDouble(Size) / Convert.ToDouble(Meta.OriginalSize))) * 100.0d);
    }
}

