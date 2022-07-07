using System;

using NewsWaffle.Models;

namespace NewsWaffle.Converter
{
	/// <summary>
    /// uses html and meta data to determine the type of page
    /// </summary>
	public static class PageClassifier
	{
		public static PageType Classify(PageMetaData metaData)
        {
			if(metaData.Type == "article")
            {
				return PageType.ContentPage;
            }

			return PageType.LinkPage;
        }
	}
}

