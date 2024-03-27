using System;
using System.Net;
using System.Text.RegularExpressions;

namespace NewsWaffle.Util;

public static class StringUtils
{
    private static readonly Regex whitespace = new Regex(@"\s+", RegexOptions.Compiled);

    public static string FormatTimeAgo(DateTime? dateTime)
        => dateTime.HasValue ? FormatTimeAgo(dateTime.Value) : "";

    public static string FormatTimeAgo(DateTime dateTime)
    {
        var s = DateTime.Now.Subtract(dateTime);
        int dayDiff = (int)s.TotalDays;

        int secDiff = (int)s.TotalSeconds;

        if (dayDiff == 0)
        {
            if (secDiff < 60)
            {
                return "just now";
            }
            if (secDiff < 120)
            {
                return "1 minute ago";
            }
            if (secDiff < 3600)
            {
                return string.Format("{0} minutes ago",
                    Math.Floor((double)secDiff / 60));
            }
            if (secDiff < 7200)
            {
                return "1 hour ago";
            }
            if (secDiff < 86400)
            {
                return string.Format("{0} hours ago",
                    Math.Floor((double)secDiff / 3600));
            }
        }
        if (dayDiff == 1)
        {
            return "yesterday";
        }
        return string.Format("{0} days ago", dayDiff);
    }

    /// <summary>
    /// normalizes a string found in HTML
    /// - HTML decodes it
    /// - strips any remaining HTML tags
    /// - converts \n, \t, and \r tabs to space
    /// - collapses runs of whitespace into a single space
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Normnalize(string? s)
    {
        if (s == null)
        {
            return "";
        }

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
