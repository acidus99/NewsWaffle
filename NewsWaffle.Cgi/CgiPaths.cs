using System;
using System.Net;
namespace NewsWaffle.Cgi
{
	public static class CgiPaths
	{
		public const string BasePath = "/cgi-bin/waffle.cgi";
		public const string ArticlePath = BasePath + "/article";
		public const string FeedPath = BasePath + "/feed";
		public const string ViewPath = BasePath + "/view";

		public static string ViewAuto(string url)
			=> $"{ViewPath}?{WebUtility.UrlEncode(url)}";

		public static string ViewArticle(string url)
			=> $"{ViewArticle}?{WebUtility.UrlEncode(url)}";

		public static string ViewFeed(string feedUrl)
			=> $"{FeedPath}?{WebUtility.UrlEncode(feedUrl)}";
	}
}

