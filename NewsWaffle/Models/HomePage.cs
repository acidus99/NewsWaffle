using System;
using System.Collections.Generic;


namespace NewsWaffle.Models
{
    public class HomePage : AbstractPage
    {
        public String Description { get; internal set; }

        public List<HyperLink> ContentLinks { get; internal set; }

        public List<HyperLink> NavigationLinks { get; internal set; }
    }
}

