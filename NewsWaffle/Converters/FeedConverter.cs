using System;
using System.Diagnostics;
using System.Linq;
using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;

using NewsWaffle.Models;
using NewsWaffle.Util;

namespace NewsWaffle.Converters
{
	public static class FeedConverter
	{
        const int DayLimit = 45;
        const int ItemLimit = 100;

		public static FeedPage ParseFeed(Uri url, string xml)
        {
            try
            {
                var timer = new Stopwatch();
                timer.Start();
                var feed = FeedReader.ReadFromString(xml);

                PageMetaData metaData = new PageMetaData
                {
                    Description = StringUtils.Normnalize(feed.Description),
                    FeaturedImage = LinkForge.Create(feed.ImageUrl),
                    OriginalSize = xml.Length,
                    SourceUrl = url,
                    ProbablyType = PageType.FeedPage,
                    Title = StringUtils.Normnalize(feed.Title),
                    SiteName = StringUtils.Normnalize(feed.Copyright),
                };
                var ret = new FeedPage(metaData)
                {
                    RootUrl = GetRootUrl(url)
                };
                ret.Links.AddRange(feed.Items.Where(x => TimeOk(x.PublishingDate)).Take(ItemLimit).Select(x => new FeedLink
                {
                    Url = new Uri(GetHtmlUrl(x)),
                    Text = StringUtils.Normnalize(x.Title),
                    Published = x.PublishingDate
                }).OrderByDescending(x=>x.Published));
                timer.Stop();
                ret.ConvertTime = (int)timer.ElapsedMilliseconds;
                return ret;
            } catch(Exception ex)
            {
                
            }
            return null;
        }

        private static Uri GetRootUrl(Uri url)
        {
            try
            {
                UriBuilder builder = new UriBuilder();
                builder.Scheme = url.Scheme;
                builder.Host = url.Host;
                builder.Port = url.Port;
                builder.Path = "/";
                return builder.Uri;
                
            } catch(Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// gets the link to the HTML article for a feed item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string GetHtmlUrl(FeedItem item)
        {
            if(item.SpecificItem is AtomFeedItem)
            {
                return GetHtmlUrl(item.SpecificItem as AtomFeedItem);
            }
            return item.Link;
        }

        /// <summary>
        /// atom feed can have multiple links tags, so find the appropriate one, with a fallback
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string GetHtmlUrl(AtomFeedItem item)
        {
            var htmlLink = item.Links.Where(x => x.Relation == "alternate" && x.LinkType == "text/html").FirstOrDefault();
            return htmlLink?.Href ?? item.Link;
        }

        private static bool TimeOk(DateTime? published)
            => (published == null) ? true :
                (DateTime.Now.Subtract(published.Value).TotalDays <= DayLimit);
	}
}

