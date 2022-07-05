using System;
using System.Linq;
using System.Collections.Generic;

using AngleSharp.Dom;

using NewsWaffle.Models;
using NewsWaffle.Converter.Special;

namespace NewsWaffle.Converter
{
    public class LinkExtractor
    {
        string NormalizedHost;
        Uri BaselUrl;

        public List<HyperLink> ContentLinks { get; internal set; }
        public List<HyperLink> NavigationLinks { get; internal set; }

        public LinkExtractor(string htmlUrl)
        {
            BaselUrl = new Uri(htmlUrl);
            NormalizedHost = BaselUrl.Host;
            if (NormalizedHost.StartsWith("www."))
            {
                NormalizedHost = NormalizedHost.Substring(4);
            }

            ContentLinks = new List<HyperLink>();
            NavigationLinks = new List<HyperLink>();

        }

        public void FindLinks(IElement content)
        {
            //first, get all the links
            var allLinks = GetLinks(content);
            //now, remove all the navigation stuff
            Preparer.RemoveMatchingTags(content, "header");
            Preparer.RemoveMatchingTags(content, "footer");
            Preparer.RemoveMatchingTags(content, "nav");
            Preparer.RemoveMatchingTags(content, "menu");

            //nav/menus are often hidden
            Preparer.RemoveMatchingTags(content, "[aria-hidden='true']");
            Preparer.RemoveMatchingTags(content, ".hidden");

            var justContent = GetLinks(content);

            ContentLinks = justContent.GetLinks();

            //now remove any content links from all links, to just get the navigation links
            allLinks.RemoveLinks(ContentLinks);
            NavigationLinks = allLinks.GetLinks();
        }


        private LinkCollection GetLinks(IElement content)
        {
            var ret = new LinkCollection();

            foreach (var link in content.QuerySelectorAll("a[href]"))
            {
                var href = link.GetAttribute("href");
                var linkText = SanitizeLinkText(link.TextContent);

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
                if (resolvedUrl == null)
                {
                    continue;
                }
                if (!resolvedUrl.Scheme.StartsWith("http"))
                {
                    continue;
                }
                if (!IsInternalLink(resolvedUrl))
                {
                    continue;
                }

                //TODO: other validation? Protocol checking? etc

                ret.AddLink(resolvedUrl, linkText);
            }
            return ret;
        }

        private bool IsInternalLink(Uri link)
            => link.Host.EndsWith(NormalizedHost);

        //
        private string SanitizeLinkText(string text)
            //remove newliens inside the text, and ensure its trimmed on both sides
            => NewlineStripper.RemoveNewlines(text).Trim();

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
