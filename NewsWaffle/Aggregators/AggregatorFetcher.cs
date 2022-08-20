using System.Diagnostics;

using NewsWaffle.Net;
using NewsWaffle.Aggregators.Models;

namespace NewsWaffle.Aggregators
{
	public static class AggregatorFetcher
	{
		public static NewsSection GetSection(string sectionName, INewAggregator aggregator)
        {
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			var url = aggregator.GetFeedUrl(sectionName);
			HttpFetcher fetcher = new HttpFetcher();
			string content = fetcher.GetAsString(url);
			stopwatch.Stop();

			var section = aggregator.ParseSection(sectionName, content, (int) stopwatch.ElapsedMilliseconds);
			section.SourceUrl = url;
			return section;
        }
	}
}
