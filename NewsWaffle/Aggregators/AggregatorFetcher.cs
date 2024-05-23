using System.Diagnostics;
using NewsWaffle.Aggregators.Models;
using CacheComms;

namespace NewsWaffle.Aggregators;

public static class AggregatorFetcher
{
    public static NewsSection GetSection(string sectionName, INewAggregator aggregator)
    {
        var url = aggregator.GetFeedUrl(sectionName);

        HttpRequestor requestor = new HttpRequestor();

        var result = requestor.GetAsString(url);
        string content = "";
        if (result && requestor.IsTextResponse)
        {
            content = requestor.BodyText;
        }
        var section = aggregator.ParseSection(sectionName, content, (int) requestor.DownloadTimeMs);
        return section;
    }
}
