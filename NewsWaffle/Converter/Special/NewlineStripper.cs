using System;
using System.Text.RegularExpressions;
namespace NewsWaffle.Converter.Special
{
	public static class NewlineStripper
	{
		private static readonly Regex whitespace = new Regex(@"\s+", RegexOptions.Compiled);

        public static string RemoveNewlines(string text)
        {
            text = text.Replace("\r", " ");
            text = text.Replace("\n", " ");
            text = whitespace.Replace(text, " ");
            return text;
        }
    }
}

