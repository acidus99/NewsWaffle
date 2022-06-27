using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace NewsWaffle.Converter
{
	/// <summary>
    /// Reads in the Raw HTML, converts it to a DOM, and strips out
    /// tags that we don't want before proper parsing
    /// </summary>
	public static class Preparer
	{
		public static IElement PrepareHtml(string wikiHtml)
		{
			//step 1: scope Html just to article content
			var contentRoot = GetContentRoot(wikiHtml);

			//step 2: remove known bad/unneeded tags
			RemoveTags(contentRoot);

            return contentRoot;
		}

        private static IElement GetContentRoot(string wikiHtml)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var parser = context.GetService<IHtmlParser>();
            var document = parser.ParseDocument(wikiHtml);
            return document.FirstElementChild;
        }

        //Removes tags we no we want need, and which make rendering harder
        //often we want to complete remove tags instead of skipping them later
        ////with the Filter, since InfoBox parser won't already visit every element
        private static void RemoveTags(IElement contentRoot)
        {
            RemoveMatchingTags(contentRoot, "style");
        }

        private static void RemoveMatchingTags(IElement element, string selector)
            => element.QuerySelectorAll(selector).ToList().ForEach(x => x.Remove());

    }
}

