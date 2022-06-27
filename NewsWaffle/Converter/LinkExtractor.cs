using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

using NewsWaffle.Models;

namespace NewsWaffle.Converter
{
    public class LinkExtractor
    {
        Uri BaselUrl;
        int LinkNumber;

        public LinkExtractor(string htmlUrl)
        {
            BaselUrl = new Uri(htmlUrl);
        }

        public List<HyperLink> GetLinks(IElement content)
        {
            List<HyperLink> ret = new List<HyperLink>();
            Dictionary<Uri, bool> alreadyAdded = new Dictionary<Uri, bool>();
            LinkNumber = 0;

            foreach (var link in content.QuerySelectorAll("a[href]"))
            {
                var href = link.GetAttribute("href");
                var text = link.TextContent.Trim();

                //we want to skip navigation hyperlinks that are just to other sections on the page
                if (href.StartsWith('#'))
                {
                    continue;
                }

                //we want to skip links without any text
                if (text.Length == 0)
                {
                    continue;
                }

                var resolvedUrl = ResolveUrl(href);
                //if it doesn't resolve, its not good
                if(resolvedUrl == null)
                {
                    continue;
                }

                //TODO: other validation? Protocol checking? etc

                //ensure its unique
                if(!alreadyAdded.ContainsKey(resolvedUrl))
                {
                    alreadyAdded.Add(resolvedUrl, true);
                    LinkNumber++;
                    ret.Add(new HyperLink
                    {
                        Number = LinkNumber,
                        Text = text,
                        Url = resolvedUrl
                    });
                }
            }
            return ret;
        }

        private Uri ResolveUrl(string href)
        {
            try
            {
                return new Uri(BaselUrl, href);
            }
            catch (Exception)
            {
            }
            return null;
        }

    }
}
