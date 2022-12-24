using System;
using System.Linq;
using System.Collections.Generic;

using AngleSharp.Dom;

using HtmlToGmi;
using HtmlToGmi.Models;
using HtmlToGmi.Special;

using NewsWaffle.Util;

namespace NewsWaffle.Converters
{
    public class LinkExtractor
    {
        string NormalizedHost;
        Uri BaselUrl;
        bool UseWordLimit = false;
        const int WordLimit = 4;

        public List<Hyperlink> ContentLinks { get; internal set; }
        public List<Hyperlink> NavigationLinks { get; internal set; }

        public string FeedUrl { get; internal set; }

        public LinkExtractor(string htmlUrl)
        {
            BaselUrl = new Uri(htmlUrl);
            NormalizedHost = BaselUrl.Host;
            if (NormalizedHost.StartsWith("www."))
            {
                NormalizedHost = NormalizedHost.Substring(4);
            }

            ContentLinks = new List<Hyperlink>();
            NavigationLinks = new List<Hyperlink>();
        }

        public void FindLinks(IElement content)
        {
            FindFeeds(content);
            //first, get all the links
            var allLinks = GetLinks(content);

            TagStripper.RemoveNavigation(content);
            UseWordLimit = true;
            var justContent = GetLinks(content);
            UseWordLimit = false;
            ContentLinks = justContent.GetLinks();

            //now remove any content links from all links, to just get the navigation links
            allLinks.RemoveLinks(ContentLinks);
            NavigationLinks = allLinks.GetLinks();
        }

        private LinkCollection GetLinks(IElement content)
        {
            var ret = new LinkCollection();

            TextExtractor textExtractor = new TextExtractor();

            foreach (var link in content.QuerySelectorAll("a[href]"))
            {

                var href = link.GetAttribute("href");
                textExtractor.Extract(link);
                var linkText = SanitizeLinkText(textExtractor.Content);

                //we want to skip navigation hyperlinks that are just to other sections on the page
                //we want to skip links without any text
                if (href.StartsWith('#') || linkText.Length == 0)
                {
                    continue;
                }

                if (UseWordLimit &&
                    linkText.Replace('&', ' ').Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Count() < WordLimit)
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
                //don't include links to self
                if(resolvedUrl == BaselUrl)
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
            //TODO: should this just use the sanitize function to handle HTML encoding?
            => HtmlToGmi.Special.StringUtils.RemoveNewlines(text).Trim();

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

        private bool FindFeeds(IElement content)
        {
            var link = content.QuerySelectorAll("link")
                .Where(x => (x.GetAttribute("rel") == "alternate") &&
                            x.HasAttribute("href") &&
                            (x.GetAttribute("type") == "application/rss+xml" ||
                             x.GetAttribute("type") == "application/atom+xml"))
                .FirstOrDefault();
            if (link != null)
            {
                var url = ResolveUrl(link.GetAttribute("href"));
                if (url != null)
                {
                    FeedUrl = url.AbsoluteUri;
                    return true;
                }
            }
            return false;
        }

      

    }
}
