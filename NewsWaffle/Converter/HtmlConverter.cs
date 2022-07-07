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
			switch (ClassifyPage())
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
		public HomePage ConvertToLinkPage()
		{
			var contentRoot = Preparer.PrepareHtml(Html);
			LinkExtractor extractor = new LinkExtractor(Url);
			extractor.FindLinks(contentRoot);
			var homePage = new HomePage
			{
				Description = MetaData.Description,
				ContentLinks = extractor.ContentLinks,
				NavigationLinks = extractor.NavigationLinks,
			};
			AssignMetadata(homePage);
			return homePage;
		}

		/// <summary>
		/// Convert to Content Page
		/// </summary>
		/// <returns></returns>
		public ArticlePage ConvertToContentPage()
		{
			var article = Reader.ParseArticle(Url, Html, null);
			var contentRoot = Preparer.PrepareHtml(article.Content);

			if(Debug)
            {
				SaveHtml("simplified.html", article.Content);
            }

			HtmlTagParser parser = new HtmlTagParser
			{
				ShouldRenderHyperlinks = false
			};
			parser.Parse(contentRoot);

			var contentItems = parser.GetItems();

			var articlePage = new ArticlePage
			{
				Byline = StringUtils.Normnalize(article.Author ?? article.Byline),
				Content = contentItems,
				Images = parser.Images,
				Links = parser.BodyLinks,
				Published = article.PublicationDate,
				SimplifiedHtml = article.Content,
				TimeToRead = article.TimeToRead,
				Title = StringUtils.Normnalize(article.Title),
				WordCount = CountWords(contentItems[0]),
			};

			AssignMetadata(articlePage);

			return articlePage;
		}

		#endregion


		#region private workings

		private void AssignMetadata(AbstractPage page)
		{
			page.FeaturedImage = MetaData.FeaturedImage;
			page.OriginalSize = Html.Length;

			if (string.IsNullOrEmpty(page.Title))
			{
				page.Title = MetaData.Title;
			}
			page.SourceUrl = MetaData.OriginalUrl;
		}

		private PageType ClassifyPage()
			=> PageClassifier.Classify(MetaData);

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
