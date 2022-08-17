using System;
using System.Net;
namespace NewsWaffle.Cgi
{
	public static class CgiPaths
	{
		public const string BasePath = "/cgi-bin/waffle.cgi";

		public static string ViewArticle(string url)
			=> $"{BasePath}/article?{WebUtility.UrlEncode(url)}";

		public static string ViewAuto(string url)
			=> $"{BasePath}/view?{WebUtility.UrlEncode(url)}";

		public static string ViewFeed(string feedUrl)
			=> $"{BasePath}/feed?{WebUtility.UrlEncode(feedUrl)}";

		public static string ViewLinks(string url)
			=> $"{BasePath}/links?{WebUtility.UrlEncode(url)}";

		public static string ViewCurrentNewsSection(string section = "")
			=> $"{BasePath}/current?{WebUtility.UrlEncode(section)}";

		public static string SwitchNewsSection
			=> $"{BasePath}/switch";
	}
}
