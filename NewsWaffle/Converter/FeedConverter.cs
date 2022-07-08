using System;
using System.Linq;
using CodeHollow.FeedReader;

using NewsWaffle.Models;
using NewsWaffle.Util;

namespace NewsWaffle.Converter
{
	public static class FeedConverter
	{
        const int DayLimit = 45;
        const int ItemLimit = 100;

		public static FeedPage ParseFeed(string url, string xml)
        {
			var feed = FeedReader.ReadFromString(xml);

            PageMetaData metaData = new PageMetaData
            {
                Description = StringUtils.Normnalize(feed.Description),
                FeaturedImage = feed.ImageUrl,
                OriginalSize = xml.Length,
                OriginalUrl = url,
                ProbablyType = PageType.FeedPage,
                Title = StringUtils.Normnalize(feed.Title),
                SiteName = StringUtils.Normnalize(feed.Copyright),
            };
            var ret = new FeedPage(metaData);
            ret.Links.AddRange(feed.Items.Where(x => TimeOk(x.PublishingDate)).Take(ItemLimit).Select(x => new FeedLink
            {
                Url = new Uri(x.Link),
                Text = x.Title,
                Published = x.PublishingDate
            }));

            return ret;
        }

        private static bool TimeOk(DateTime? published)
            => (published == null) ? true :
                (DateTime.Now.Subtract(published.Value).TotalDays <= DayLimit);
	}
}

