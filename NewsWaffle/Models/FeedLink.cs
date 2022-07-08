using System;
namespace NewsWaffle.Models
{
	public class FeedLink : HyperLink
	{
		public DateTime? Published { get; set; }
        public string TimeAgo
            => PrettyTimeAgo();

        public bool HasPublished => Published.HasValue;

        string PrettyTimeAgo()
        {
            if(Published == null)
            {
                return "";
            }

            var s = DateTime.Now.Subtract(Published.Value);
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
    }
}

