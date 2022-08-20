using System;

using NewsWaffle.Util;

namespace NewsWaffle.Models
{
	public class FeedLink : HyperLink
	{
		public DateTime? Published { get; set; }
        public string TimeAgo
            => StringUtils.FormatTimeAgo(Published);

        public bool HasPublished => Published.HasValue;
    }
}