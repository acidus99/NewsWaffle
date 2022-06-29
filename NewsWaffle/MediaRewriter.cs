using System;
using System.Net;
namespace NewsWaffle
{
	public static class MediaRewriter
	{
		public static string Proxy { get; set; } = "";

		public static string GetPath(string url)
			=> (Proxy.Length > 0) ? $"{Proxy}?{WebUtility.UrlEncode(url)}" : url;
	}
}

