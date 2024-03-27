using System.Diagnostics;
using NewsWaffle.Aggregators.Models;
using NewsWaffle.Net;

namespace NewsWaffle.Aggregators;

public static class AggregatorFetcher
{
    public static NewsSection GetSection(string sectionName, INewAggregator aggregator)
    {
        Stopwatch stopwatch = new Stopwatch();
        var url = aggregator.GetFeedUrl(sectionName);

        IHttpRequestor requestor = new HttpRequestor();
        stopwatch.Start();
        var result = requestor.Request(url);
        string content = "";
        if (result)
        {
            content = requestor.BodyText;
        }
        stopwatch.Stop();
        var section = aggregator.ParseSection(sectionName, content, (int)stopwatch.ElapsedMilliseconds);
        section.SourceUrl = url;
        return section;
    }
}
