using System;
using System.Net;
namespace NewsWaffle
{
	public static class LinkRewriter
	{
		public static string ImageProxy { get; set; } = "";
        public static string LinkProxy { get; set; } = "";

        public static string GetImageUrl(Uri url)
            => ProxyLink(ImageProxy, url);

        public static string GetLinkUrl(Uri url)
            => ProxyLink( LinkProxy, url);

        private static string ProxyLink(string proxy, Uri url)
        {
            if(proxy.Length > 0 && ShouldProxyUrl(url))
            {
                return $"{proxy}?{WebUtility.UrlEncode(url.AbsoluteUri)}";
            }
            return url.AbsoluteUri;
        }

        private static bool ShouldProxyUrl(Uri url)
            => (url.Scheme == "http") || (url.Scheme == "https");
    }
}

