using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using AngleSharp.Dom;

using NewsWaffle.Models;
using NewsWaffle.Converter.Special;

namespace NewsWaffle.Converter
{
    public class LinkExtractor
    {
        //used to collapse runs of whitespace
        private static readonly Regex whitespace = new Regex(@"\s+", RegexOptions.Compiled);

        string NormalizedHost;
        Uri BaselUrl;

        public List<HyperLink> ContentLinks { get; internal set; }
        public List<HyperLink> NavigationLinks { get; internal set; }

        public LinkExtractor(string htmlUrl)
        {
            BaselUrl = new Uri(htmlUrl);
            NormalizedHost = BaselUrl.Host;
            if(NormalizedHost.StartsWith("www."))
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

            //nav menus are often hidden
            Preparer.RemoveMatchingTags(content, "[aria-hidden='true']");

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

        /// <summary>
        /// Smart collection to handle links
        /// - Only adds a Url if we haven't seen it before
        /// - Only adds a Url if we haven't seen the link text before
        /// - To improve quality of link text, if 2 links point to the same URL, uses the link text which is longer 
        /// </summary>
        private class LinkCollection
        {
            Dictionary<Uri, HyperLink> links = new Dictionary<Uri, HyperLink>();

            Dictionary<string, bool> seenText = new Dictionary<string, bool>();

            int counter = 0;

            public void AddLink(Uri url, string linkText) 
            {
                string normalized = linkText.ToLower();

                //does this Url already exist?
                if (!links.ContainsKey(url))
                {
                    //has this link text already been used?
                    if (!seenText.ContainsKey(normalized))
                    {

                        seenText.Add(normalized, true);
                        //add it
                        counter++;
                        links.Add(url, new HyperLink
                        {
                            Text = linkText,
                            Url = url,
                            OrderDetected = counter
                        });
                    }
                }
                else if (!seenText.ContainsKey(normalized))
                {
                    //URL already exists, but link text doesn't.
                    //so is this link text "better" that what we are using?
                    if (links[url].Text.Length < linkText.Length)
                    {
                        links[url].Text = linkText;
                        seenText.Add(normalized, true);
                    }
                }
            }

            public void RemoveLinks(List<HyperLink> linksToRemove)
                => linksToRemove.ForEach(x => links.Remove(x.Url));

            public List<HyperLink> GetLinks()
                => links.Values.OrderBy(x => x.OrderDetected).ToList();
        }
    }
}
