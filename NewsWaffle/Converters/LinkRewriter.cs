using System;
using System.Net;
namespace NewsWaffle
{
	public static class LinkRewriter
	{
		public static string ImageProxy { get; set; } = "";
        public static string LinkProxy { get; set; } = "";

        public static string GetImageUrl(string url)
            => ProxyLink(ImageProxy, url);

        public static string GetImageUrl(Uri url)
            => GetImageUrl(url.AbsoluteUri);

        public static string GetLinkUrl(string url)
            => ProxyLink(LinkProxy, url);

        private static string ProxyLink(string proxy, string url)
            => (proxy.Length > 0) ? $"{proxy}?{WebUtility.UrlEncode(url)}" : url;

    }
}

