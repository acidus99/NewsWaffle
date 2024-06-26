﻿using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using HtmlToGmi;
using HtmlToGmi.Models;

namespace NewsWaffle.Converters;

public class LinkParser : AbstractParser
{
    string NormalizedHost;
    const int WordLimit = 4;
    public int LinkTextMaxLength = 120;

    public List<Hyperlink> ContentLinks { get; internal set; }
    public List<Hyperlink> NavigationLinks { get; internal set; }

    public Uri FeedUrl { get; internal set; }

    public LinkParser(Uri htmlUrl)
        : base(htmlUrl)
    {
        NormalizedHost = BaseUrl.Host;
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
        var justContent = GetLinks(content, true, true);
        ContentLinks = justContent.GetLinks();

        //now remove any content links from all links, to just get the navigation links
        allLinks.RemoveLinks(ContentLinks);
        NavigationLinks = allLinks.GetLinks();
    }

    private LinkCollection GetLinks(IElement content, bool useWordLimit = false, bool limitLinkText = false)
    {
        var ret = new LinkCollection();

        TextConverter textExtractor = new TextConverter();

        foreach (var link in content.QuerySelectorAll("a[href]"))
        {
            var href = link.GetAttribute("href");
            //we want to skip navigation hyperlinks that are just to other sections on the page
            if (href.StartsWith('#'))
            {
                continue;
            }
            var linkText = SanitizeLinkText(textExtractor.Convert(link));

            //we want to skip links without any text
            if (linkText.Length == 0)
            {
                continue;
            }

            if (useWordLimit &&
                linkText.Replace('&', ' ').Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Count() < WordLimit)
            {
                continue;
            }

            //TODO: maybe do some stuff to better normalize the URL
            //reove fragments, maybe strip query string of stuff like
            //google analytics tracking 
            var resolvedUrl = CreateHttpUrl(href);

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
            if(resolvedUrl == BaseUrl)
            {
                continue;
            }

            //TODO: other validation? Protocol checking? etc

            if(limitLinkText)
            {
                linkText = TrimLinkText(linkText);
            }

            ret.AddLink(resolvedUrl, linkText, false);
        }
        return ret;
    }

    private string TrimLinkText(string linkText)
    {
        //first trim to no more than 2 sentences
        var sentences = linkText.Split(". ", 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if(sentences.Length == 2 && sentences[0].Length < LinkTextMaxLength * 0.9d)
        {
            linkText = $"{sentences[0]}. {sentences[1]}";
        } else
        {
            linkText = sentences[0];
        }
        //now trim length
        if(linkText.Length > LinkTextMaxLength)
        {
            //trim it
            linkText = linkText.Substring(0, LinkTextMaxLength);
            //now trim to the last space
            if (linkText.Contains(' '))
            {
                linkText = linkText.Substring(0, linkText.LastIndexOf(' '));
            }

            linkText += '…';
        }
        return linkText;
    }

    private bool IsInternalLink(Uri link)
        => link.Host.EndsWith(NormalizedHost);

    private string SanitizeLinkText(string text)
        //remove newliens inside the text, and ensure its trimmed on both sides
        //TODO: should this just use the sanitize function to handle HTML encoding?
        => CollapseWhitespace(text).Trim();

    private void FindFeeds(IElement content)
    {
        var link = content.QuerySelectorAll("link")
            .Where(x => (x.GetAttribute("rel") == "alternate") &&
                        x.HasAttribute("href") &&
                        (x.GetAttribute("type") == "application/rss+xml" ||
                         x.GetAttribute("type") == "application/atom+xml"))
            .FirstOrDefault();
        if (link != null)
        {
            FeedUrl = CreateHttpUrl(link.GetAttribute("href"));
        }
    }
}
