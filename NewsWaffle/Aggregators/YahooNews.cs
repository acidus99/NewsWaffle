﻿using System;
using System.Diagnostics;

using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;

using NewsWaffle.Aggregators.Models;

namespace NewsWaffle.Aggregators
{
	public class YahooNews : INewAggregator
	{
        public string Name => "Yahoo News";

        public string DefaultSection => "World";

        public string[] AvailableSections => new string[]
        {
            "Business",
            "Entertainment",
            "Headlines",
            "Health",
            "Science",
            "Sports",
            "Technology",
            "World",
        };

        public string GetFeedUrl(string sectionName)
        {
            switch(sectionName)
            {
                case "Business":
                    return "https://news.yahoo.com/rss/business";
                case "Entertainment":
                    return "https://news.yahoo.com/rss/entertainment";

                case "Headlines":
                    return "https://news.yahoo.com/rss/";

                case "Health":
                    return "https://news.yahoo.com/rss/health";

                case "Science":
                    return "https://news.yahoo.com/rss/science";

                case "Sports":
                    return "https://news.yahoo.com/rss/sports";

                case "Technology":
                    return "https://news.yahoo.com/rss/tech";

                case "World":
                    return "https://news.yahoo.com/rss/world";

                default:
                    return null;
            }
        }

        public NewsSection ParseSection(string sectionName, string content, int downloadTime)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var section = new NewsSection(sectionName, Name)
            {
                OriginalSize = content.Length,
                DownloadTime = downloadTime,
            };

            var feed = FeedReader.ReadFromString(content);
            foreach(var item in feed.Items)
            {
                var story = ParseStory(item.SpecificItem as MediaRssFeedItem);
                if(story != null)
                {
                    section.Stories.Add(story);
                }
            }
            stopwatch.Stop();
            section.ConvertTime = (int)stopwatch.ElapsedMilliseconds;

            return section;
        }

        private NewsStory ParseStory(MediaRssFeedItem item)
            => new NewsStory
            {
                Source = item.Source.Value,
                Title = item.Title,
                Updated = item.PublishingDate ?? DateTime.MinValue,
                Url = item.Link
            };

        public bool IsValidSection(string sectionName)
            => GetFeedUrl(sectionName) != null;
    }
}