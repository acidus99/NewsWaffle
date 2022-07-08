using System;
using System.Linq;

using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
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
		Lazy<HtmlHead> head;


		public PageMetaData GetMetaData(string url, string html)
		{
			openGraph = OpenGraph.ParseHtml(html);
			head = new Lazy<HtmlHead>(() => new HtmlHead(html));

			var TMP = new HtmlHead(html);


			return new PageMetaData
			{
				Description = GetDescription(),
				FeaturedImage = GetFeatureImage(),
				OriginalSize = html.Length,
				OriginalUrl = url,
				ProbablyType = ClassifyPageType(),
				SiteName = GetSiteName(),
				Title = GetTitle(),				
			};
		}

		private string GetDescription()
			=> StringUtils.Normnalize(openGraph.Metadata["og:description"].FirstOrDefault()?.Value ?? "");

		private string GetFeatureImage()
			=> openGraph.Image?.AbsoluteUri ?? null;

		private string GetSiteName()
		{ 
			var name =  openGraph.Metadata["og:site_name"].FirstOrDefault()?.Value ?? "";
			if(name is "")
            {
				name = head.Value.ApplicationName;
            }
			return StringUtils.Normnalize(name);
		}

		private string GetTitle()
			=> StringUtils.Normnalize(openGraph.Title is "" ? head.Value.Title : openGraph.Title);

		private PageType ClassifyPageType()
        {
			if(openGraph.Type == "article" || head.Value.HasArticleProperties)
            {
				return PageType.ContentPage;
            }
			return PageType.LinkPage;
        }

		private class HtmlHead
		{
			public IElement Head { get; private set; }

			public HtmlHead(string html)
			{
				var context = BrowsingContext.New(Configuration.Default);
				var parser = context.GetService<IHtmlParser>();
				var document = parser.ParseDocument(html);
				Head = document.QuerySelector("head");
			}

			public string Title
				=> Head.QuerySelector("title")?.TextContent ?? "";

			public string ApplicationName
				=> Head.QuerySelectorAll("meta")
					.Where(x => (x.GetAttribute("name") == "application-name"))
					.FirstOrDefault()?.GetAttribute("content") ?? "";

			public bool HasArticleProperties
				=> Head.QuerySelectorAll("meta")
					.Where(x => (x.GetAttribute("property")?.StartsWith("article:") ?? false )).Count() >= 4;

		}

	}
}