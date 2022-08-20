using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SmartReader;

using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

using NewsWaffle.Models;
using NewsWaffle.Util;

namespace NewsWaffle.Converter
{
    public class HtmlConverter
    {

		public bool Debug { get; set; } = true;

		string Url;
		string Html;
		IElement document;
		PageMetaData MetaData;
		Stopwatch timer = new Stopwatch();

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
			timer.Start();
			EnsureParsed();
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
			if(!timer.IsRunning)
            {
				timer.Start();
            }
			EnsureParsed();
			LinkExtractor extractor = new LinkExtractor(Url);
			extractor.FindLinks(document);
			var homePage = new LinkPage(MetaData)
			{
				ContentLinks = extractor.ContentLinks,
				NavigationLinks = extractor.NavigationLinks,
				FeedUrl = extractor.FeedUrl,
			};
			timer.Stop();
			homePage.ConvertTime = (int) timer.ElapsedMilliseconds;
			return homePage;
		}

		/// <summary>
		/// Convert to Content Page
		/// </summary>
		/// <returns></returns>
		public ContentPage ConvertToContentPage()
		{
			if (!timer.IsRunning)
			{
				timer.Start();
			}
			EnsureParsed();
			var article = Reader.ParseArticle(Url, Html, null);
			ContentPage page = null;

			if (article.IsReadable && article.Content != "")
			{
				if (Debug)
				{
					SaveHtml("simplified.html", article.Content);
				}

				HtmlTagParser parser = new HtmlTagParser
				{
					ShouldRenderHyperlinks = false
				};
				parser.Parse(ParseToRoot(article.Content));

				var contentItems = parser.GetItems();

				page = new ContentPage(MetaData)
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

				page.Meta.Title = StringUtils.Normnalize(article.Title);
			}
			else
			{
				page = new ContentPage(MetaData)
				{
					IsReadability = false,
					Excerpt = StringUtils.Normnalize(article.Excerpt)
				};
			}
			timer.Stop();
			page.ConvertTime = (int)timer.ElapsedMilliseconds;
			return page;
		}

		#endregion

		#region private workings

		private void EnsureParsed()
        {
			if(document == null)
            {
				document = ParseToRoot(Html);
            }
			if(MetaData == null)
            {
				ParseMetadata();
			}
        }

		private int CountWords(ContentItem content)
			=> content.Content.Split("\n").Where(x => !x.StartsWith("=> ")).Sum(x => CountWords(x));

		private int CountWords(string s)
			=> s.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;


		private void ParseMetadata()
		{
			MetaDataParser parser = new MetaDataParser();
			MetaData = parser.GetMetaData(Url, Html, document);
		}

		private void SaveHtml(string filename, string html)
			=> File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/tmp/" + filename, html);

		private IElement ParseToRoot(string html)
		{
			var context = BrowsingContext.New(Configuration.Default);
			var parser = context.GetService<IHtmlParser>();
			var document = parser.ParseDocument(html);
			return document.FirstElementChild;
		}


		#endregion
	}
}
