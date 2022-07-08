using System;
namespace NewsWaffle.Models
{
	public class PageMetaData
	{
		public string Description { get; internal set; }
		public string FeaturedImage { get; internal set; }
		public int OriginalSize { get; internal set; }
		public string OriginalUrl { get; internal set; }
		public string SiteName { get; internal set; }
		public string Title { get; internal set; }
		public PageType ProbablyType { get; internal set; }

	}
}

