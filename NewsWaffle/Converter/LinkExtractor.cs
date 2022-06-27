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
        string NormalizedHost;
        Uri BaselUrl;
        int LinkNumber;
        Dictionary<Uri, bool> AlreadyAddedUrls;
        Dictionary<string, bool> AlreadyAddedLinkText;

        public LinkExtractor(string htmlUrl)
        {
            BaselUrl = new Uri(htmlUrl);
            NormalizedHost = BaselUrl.Host;
            if(NormalizedHost.StartsWith("www."))
            {
                NormalizedHost = NormalizedHost.Substring(4);
            }
            AlreadyAddedUrls = new Dictionary<Uri, bool>();
            AlreadyAddedLinkText = new Dictionary<string, bool>();
        }

        public List<HyperLink> GetLinks(IElement content)
        {
            List<HyperLink> ret = new List<HyperLink>();
            AlreadyAddedUrls.Clear();
            AlreadyAddedLinkText.Clear();
            LinkNumber = 0;

            foreach (var link in content.QuerySelectorAll("a[href]"))
            {
                var href = link.GetAttribute("href");
                var linkText = link.TextContent.Trim();

                //we want to skip navigation hyperlinks that are just to other sections on the page
                //we want to skip links without any text
                if (href.StartsWith('#') || linkText.Length == 0)
                {
                    continue;
                }

                //TODO: maybe do some stuff to better normalize the URL
                //reove fragments, maybe strip query string of stuff like
                //google analytics tracking 
                var resolvedUrl = ResolveUrl(href);

                //if it doesn't resolve, its not good
                if(resolvedUrl == null)
                {
                    continue;
                }
                if(!resolvedUrl.Scheme.StartsWith("http"))
                {
                    continue;
                }
                if(!IsInternalLink(resolvedUrl))
                {
                    continue;
                }

                //TODO: other validation? Protocol checking? etc

                var normalized = NormalizeLinkText(linkText);
                //ensure the URL and linktext are unique
                if(!AlreadyAddedUrls.ContainsKey(resolvedUrl) &&
                    !AlreadyAddedLinkText.ContainsKey(normalized))
                {
                    AlreadyAddedLinkText.Add(normalized, true);
                    AlreadyAddedUrls.Add(resolvedUrl, true);
                    LinkNumber++;
                    ret.Add(new HyperLink
                    {
                        Number = LinkNumber,
                        Text = linkText,
                        Url = resolvedUrl
                    });
                }
            }
            return ret;
        }

        private bool IsInternalLink(Uri link)
            => link.Host.EndsWith(NormalizedHost);

        private string NormalizeLinkText(string text)
            => text.ToLower();

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
