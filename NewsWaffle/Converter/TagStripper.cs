using System.Linq;

using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace NewsWaffle.Converter
{
	/// <summary>
    /// Removes specific tags from a DOM tree to add in parsing/extracting
    /// </summary>
	public static class TagStripper
	{

		public static void RemoveProblemTags(IElement element)
        {
			RemoveMatchingTags(element, "svg");
		}

		public static void RemoveNavigation(IElement element)
        {
            //now, remove all the navigation stuff
            RemoveMatchingTags(element, "header");
            RemoveMatchingTags(element, "footer");
            RemoveMatchingTags(element, "nav");
            RemoveMatchingTags(element, "menu");

            //nav/menus are often hidden
            RemoveMatchingTags(element, "[aria-hidden='true']");
            RemoveMatchingTags(element, ".hidden");
        }

		private static void RemoveMatchingTags(IElement element, string selector)
		=> element.QuerySelectorAll(selector).ToList().ForEach(x => x.Remove());

	}
}

