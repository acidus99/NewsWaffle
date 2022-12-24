using System;
using System.Net;
namespace NewsWaffle.Cgi
{
	public static class CgiPaths
	{
		public const string BasePath = "/cgi-bin/waffle.cgi";

		public const string MediaProxyEndpoint = BasePath + "/media.jpg";

        public const string AutoViewEndpoint = BasePath + "/view";

		public static string ViewArticle(Uri url)
			=> $"{BasePath}/article?{WebUtility.UrlEncode(url.AbsoluteUri)}";

		public static string ViewAuto(Uri url)
			=> $"{BasePath}/view?{WebUtility.UrlEncode(url.AbsoluteUri)}";

		public static string ViewFeed(Uri feedUrl)
			=> $"{BasePath}/feed?{WebUtility.UrlEncode(feedUrl.AbsoluteUri)}";

		public static string ViewLinks(Uri url)
			=> $"{BasePath}/links?{WebUtility.UrlEncode(url.AbsoluteUri)}";

		public static string ViewRaw(Uri url)
				=> $"{BasePath}/raw?{WebUtility.UrlEncode(url.AbsoluteUri)}";

		public static string ViewCurrentNewsSection(string section = "")
			=> $"{BasePath}/current/yahoo?{WebUtility.UrlEncode(section)}";

		public static string SwitchNewsSection
			=> $"{BasePath}/switch/yahoo";
	}
}
