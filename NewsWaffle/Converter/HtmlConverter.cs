using System;
using System.IO;
using System.Linq;
using SmartReader;

using NewsWaffle.Models;
using NewsWaffle.Util;

namespace NewsWaffle.Converter
{
    public class HtmlConverter
    {

		public bool Debug { get; set; } = true;

		string Url;
		string Html;
		PageMetaData MetaData;

		public HtmlConverter(string url, string html)
		{
			Url = url;
			Html = html;
			if (Debug)
			{
				SaveHtml("original.html", Html);
			}
		}

		#region public methods

		/// <summary>
		/// Convert the page, auto-detecting the type
		/// </summary>
		public AbstractPage Convert()
		{
			ParseMetadata();
			switch (MetaData.ProbablyType)
			{
				case PageType.ContentPage:
					return ConvertToContentPage();

				default:
					return ConvertToLinkPage();
			}
		}

		/// <summary>
		/// Convert the Link Page
		/// </summary>
		public LinkPage ConvertToLinkPage()
		{
			if (MetaData == null)
			{
				ParseMetadata();
			}
			var contentRoot = Preparer.PrepareHtml(Html);
			LinkExtractor extractor = new LinkExtractor(Url);
			extractor.FindLinks(contentRoot);
			var homePage = new LinkPage(MetaData)
			{
				ContentLinks = extractor.ContentLinks,
				NavigationLinks = extractor.NavigationLinks,
				FeedUrl = extractor.FeedUrl,
			};
			return homePage;
		}

		/// <summary>
		/// Convert to Content Page
		/// </summary>
		/// <returns></returns>
		public ContentPage ConvertToContentPage()
		{
			if(MetaData == null)
            {
				ParseMetadata();
            }
			var article = Reader.ParseArticle(Url, Html, null);

			if (article.IsReadable)
			{

				var contentRoot = Preparer.PrepareHtml(article.Content);

				if (Debug)
				{
					SaveHtml("simplified.html", article.Content);
				}

				HtmlTagParser parser = new HtmlTagParser
				{
					ShouldRenderHyperlinks = false
				};
				parser.Parse(contentRoot);

				var contentItems = parser.GetItems();

				var articlePage = new ContentPage(MetaData)
				{
					IsReadability = true,
					Byline = StringUtils.Normnalize(article.Author ?? article.Byline),
					Content = contentItems,
					Images = parser.Images,
					Links = parser.BodyLinks,
					Published = article.PublicationDate,
					TimeToRead = article.TimeToRead,
					WordCount = CountWords(contentItems[0]),
				};

				articlePage.Meta.Title = StringUtils.Normnalize(article.Title);

				return articlePage;
			}
			else
			{
				return new ContentPage(MetaData)
				{
					IsReadability = false,
					Excerpt = StringUtils.Normnalize(article.Excerpt)
				};
			}
		}

		#endregion


		#region private workings

		private int CountWords(ContentItem content)
			=> content.Content.Split("\n").Where(x => !x.StartsWith("=> ")).Sum(x => CountWords(x));

		private int CountWords(string s)
			=> s.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;


		private void ParseMetadata()
		{
			MetaDataParser parser = new MetaDataParser();
			MetaData = parser.GetMetaData(Url, Html);
		}

		private void SaveHtml(string filename, string html)
			=> File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/tmp/" + filename, html);

		#endregion
	}
}
