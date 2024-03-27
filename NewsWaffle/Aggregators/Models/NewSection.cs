using System;
using System.Collections.Generic;
using System.Linq;
using NewsWaffle.Models;

namespace NewsWaffle.Aggregators.Models;

public class NewsSection : IPageStats
{
    public string AggregatorName { get; private set; }

    public string Copyright => AggregatorName;

    public string SectionName { get; private set; }

    public List<NewsStory> Stories { get; private set; }

    public int DownloadTime { get; set; }

    public int ConvertTime { get; set; }

    public int OriginalSize { get; set; }

    public Uri SourceUrl { get; set; }

    public int Size
    => Stories.Sum(x => x.Size) + SectionName.Length + AggregatorName.Length + 20;

    internal NewsSection(string sectionName, string aggregatorName)
    {
        SectionName = sectionName;
        AggregatorName = aggregatorName;
        Stories = new List<NewsStory>();

        ConvertTime = 0;
        DownloadTime = 0;
        OriginalSize = 0;
        SourceUrl = null;
    }
}

