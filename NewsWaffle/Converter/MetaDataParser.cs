using System;
using System.Linq;
using OpenGraphNet;
using NewsWaffle.Models;
using NewsWaffle.Util;

namespace NewsWaffle.Converter
{
	/// <summary>
    /// Extracts meta data from the HTML
    /// </summary>
	public class MetaDataParser
	{
		OpenGraph openGraph;

		public PageMetaData GetMetaData(string url, string html)
		{
			openGraph = OpenGraph.ParseHtml(html);

			return new PageMetaData
			{
				Description = GetDescription(),
				FeaturedImage = GetFeatureImage(),
				OriginalUrl = url,
				Title = GetTitle(),
				Type = GetOpenGraphType()
			};
		}

		private string GetDescription()
			=> StringUtils.Normnalize(openGraph.Metadata["og:description"].FirstOrDefault()?.Value ?? "");

		private string GetFeatureImage()
			=> openGraph.Image?.AbsoluteUri ?? null;

		private string GetTitle()
			//TODO use HTML as well
			=> StringUtils.Normnalize(openGraph.Title ?? "");

		private string GetOpenGraphType()
			=> openGraph.Type;

	}
}

