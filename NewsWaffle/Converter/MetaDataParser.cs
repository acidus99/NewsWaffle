using System;
using System.Linq;

using AngleSharp.Dom;

using NewsWaffle.Models;
using NewsWaffle.Util;

namespace NewsWaffle.Converter
{
	/// <summary>
    /// Extracts meta data from the HTML
    /// </summary>
	public class MetaDataParser
	{
		HtmlHead head;

		public PageMetaData GetMetaData(string url, string html, IElement document)
		{
			head = new HtmlHead(document);
			return new PageMetaData
			{
				Description = GetDescription(),
				FeaturedImage = GetFeatureImage(),
				OriginalSize = html.Length,
				OriginalUrl = url,
				ProbablyType = ClassifyPageType(),
				SiteName = GetSiteName(url),
				Title = GetTitle(),				
			};
		}

		private string GetDescription()
			=> StringUtils.Normnalize(head.OGDescription);

		private string GetFeatureImage()
			=> head.OGImage ?? null;

		private string GetSiteName(string url)
		{ 
			var name =  head.OGSiteName ?? "";
			if(name is "")
            {
				name = head.ApplicationName;
            }
			if(name is "")
            {
				name = (new Uri(url)).Host.Replace("www.", "");
            }
			return StringUtils.Normnalize(name);
		}

		private string GetTitle()
			=> StringUtils.Normnalize(string.IsNullOrEmpty(head.OGTitle) ? head.Title : head.OGTitle);

		private PageType ClassifyPageType()
        {
			if(StringUtils.Normnalize(head.OGType) == "article" || head.HasArticleProperties)
            {
				return PageType.ContentPage;
            }
			return PageType.LinkPage;
        }

		private class HtmlHead
		{
			public IElement Head { get; private set; }

			public HtmlHead(IElement document)
			{
				Head = document.QuerySelector("head");
				foreach(var element in Head.QuerySelectorAll("meta[property]"))
                {
					string content = element.GetAttribute("content") ?? "";

					switch(element.GetAttribute("property").ToLower())
                    {
						case "og:image":
							OGImage = content;
							break;
						case "og:site_name":
							OGSiteName = content;
							break;
						case "og:title":
							OGTitle = content;
							break;
						case "og:type":
							OGType = content;
							break;
						case "og:description":
							OGDescription = content;
							break;
                    }
                }
			}

			public string OGTitle { get; set; }
			public string OGType { get; set; }
			public string OGImage { get; set; }
			public string OGSiteName { get; set; }
			public string OGDescription { get; set; }

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