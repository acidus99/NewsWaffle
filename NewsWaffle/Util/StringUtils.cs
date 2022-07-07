using System;
using System.Net;
using System.Text.RegularExpressions;

namespace NewsWaffle.Util
{
	public static class StringUtils
	{
        private static readonly Regex whitespace = new Regex(@"\s+", RegexOptions.Compiled);

        /// <summary>
        /// normalizes a string found in HTML
        /// - HTML decodes it
        /// - strips any remaining HTML tags
        /// - converts \n, \t, and \r tabs to space
        /// - collapses runs of whitespace into a single space
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Normnalize(string s)
        {
            //decode
            s = WebUtility.HtmlDecode(s);
            //strip tags
            s = Regex.Replace(s, @"<[^>]*>", "");
            if (s.Contains('\t'))
            {
                s.Replace('\t', ' ');
            }
            return RemoveNewlines(s);
        }

        public static string RemoveNewlines(string text)
        {
            if (text.Length > 0 && (text.Contains('\n') || text.Contains('\r')))
            {
                text = text.Replace('\r', ' ');
                text = text.Replace('\n', ' ');
                text = whitespace.Replace(text, " ");
            }
            return text;
        }
    }
}
