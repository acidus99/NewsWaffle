using System;
namespace NewsWaffle.Aggregators.Models
{
	public class NewsStory
	{
		public string Title { get; set; }

		public string Url { get; set; }

		public string Source { get; set; }

		public DateTime Updated { get; set; }
	}
}

