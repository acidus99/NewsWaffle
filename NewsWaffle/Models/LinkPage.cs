using System;
using System.Linq;
using System.Collections.Generic;


namespace NewsWaffle.Models
{
    public class LinkPage : AbstractPage
    {
        public String Description { get; internal set; }

        public List<HyperLink> ContentLinks { get; internal set; }

        public List<HyperLink> NavigationLinks { get; internal set; }

        public override int Size
            => ContentLinks.Sum(x => x.Size) +
                NavigationLinks.Sum(x => x.Size) +
                Description.Length +
                Title.Length + 30;
    }
}

