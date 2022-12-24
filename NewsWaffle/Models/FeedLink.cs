using System;

using NewsWaffle.Util;
using HtmlToGmi.Models;


namespace NewsWaffle.Models
{
	public class FeedLink : Hyperlink
	{
		public DateTime? Published { get; set; }
        public string TimeAgo
            => StringUtils.FormatTimeAgo(Published);

        public bool HasPublished => Published.HasValue;
    }
}