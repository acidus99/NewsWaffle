using System;

using NewsWaffle.Net;
using NewsWaffle.Aggregators.Models;

namespace NewsWaffle.Aggregators
{
	public static class AggregatorFetcher
	{
		public static Section GetSection(string sectionName, INewAggregator aggregator)
        {
			var url = aggregator.GetFeedUrl(sectionName);
			HttpFetcher fetcher = new HttpFetcher();
			string content = fetcher.GetAsString(url);
			return aggregator.ParseSection(sectionName, content);
        }
	}
}
