using System;

using NewsWaffle.Util;

namespace NewsWaffle.Aggregators.Models
{
	public class NewsStory
	{
		public string Title { get; set; }

		public Uri Url { get; set; }

		public string Source { get; set; }

		public DateTime Updated { get; set; }

		public string TimeAgo
			=> StringUtils.FormatTimeAgo(Updated);

		//20 is roughly the overhead per link
		public int Size => Title.Length + Url.AbsoluteUri.Length + Source.Length + 20;
	}
}

