using System;
using System.Linq;
using System.Collections.Generic;

namespace NewsWaffle.Models
{
	public class FeedPage : AbstractPage
	{
		public List<FeedLink> Links = new List<FeedLink>();
        public string RootUrl { get; internal set; }

        public FeedPage(PageMetaData metaData)
            : base(metaData)
        {
        }

        public override int Size =>  Links.Sum(x => x.Size) +
                Meta.Description.Length +
                Meta.Title.Length + 30;
    }
}

