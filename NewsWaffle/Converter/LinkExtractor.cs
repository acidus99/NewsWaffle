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

        public List<HyperLink> GetLinks(string htmlUrl, IElement content)
        {
            List<HyperLink> ret = new List<HyperLink>();
            foreach(var link in content.QuerySelectorAll("a[href]"))
            {
                var href = link.GetAttribute("href");
                var text = link.TextContent.Trim();

                if (text.Length > 0)
                {
                    Uri resolvedUrl = null;
                    try
                    {
                        Uri baseUrl = new Uri(htmlUrl);
                        resolvedUrl = new Uri(baseUrl, href);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    ret.Add(new HyperLink
                    {
                        Text = text,
                        Url = resolvedUrl
                    });
                }
            }
            return ret;
        }
    }
}
