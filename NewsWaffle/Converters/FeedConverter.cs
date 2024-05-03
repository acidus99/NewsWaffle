using System;
using System.Diagnostics;
using System.Linq;
using CodeHollow.FeedReader;
using NewsWaffle.Models;
using NewsWaffle.Util;

namespace NewsWaffle.Converters;

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
            var sourceFeed = FeedReader.ReadFromString(xml);

            PageMetaData metaData = new PageMetaData
            {
                Description = StringUtils.Normnalize(sourceFeed.Description),
                FeaturedImage = LinkForge.Create(sourceFeed.ImageUrl),
                OriginalSize = xml.Length,
                SourceUrl = url,
                ProbablyType = PageType.FeedPage,
                Title = StringUtils.Normnalize(sourceFeed.Title),
                SiteName = StringUtils.Normnalize(sourceFeed.Copyright),
            };
            var ret = new FeedPage(metaData)
            {
                RootUrl = GetRootUrl(url)
            };

            var actualConverter = new HtmlToGmi.NewsFeeds.FeedConverter();
            HtmlToGmi.NewsFeeds.Feed parsedFeed = actualConverter.Convert(sourceFeed);

            ret.Items.AddRange(parsedFeed.Items
                .Where(x => ShouldIncludeFeedItem(x))
                .Take(ItemLimit)
                .OrderByDescending(x => x.Published));

            timer.Stop();
            ret.ConvertTime = (int)timer.ElapsedMilliseconds;
            return ret;
        }
        catch (Exception)
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
        }
        catch (Exception)
        {
        }
        return null;
    }

    private static bool ShouldIncludeFeedItem(HtmlToGmi.NewsFeeds.FeedItem feedItem)
        => (!feedItem.Published.HasValue)
            ? true :
            //abs allows this to show items in the future
            (Math.Abs(DateTime.Now.Subtract(feedItem.Published.Value).TotalDays) <= DayLimit);
}
