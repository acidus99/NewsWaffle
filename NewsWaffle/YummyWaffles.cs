using System;

using NewsWaffle.Net;
using NewsWaffle.Converter;
using NewsWaffle.Models;

namespace NewsWaffle
{
    public class YummyWaffles
    {

        public AbstractPage Page { get; internal set; } = null;

        public string ErrorMessage { get; internal set; } = "";

        public bool GetPage(string url, bool forceArticle = false)
        {
            try
            {
                //========= Step 1: Get HTML
                var fetcher = new HttpFetcher();
                var html = fetcher.GetHtml(url);

                //========= Step 2: Parse it to a type
                var converter = new HtmlConverter();
                if (forceArticle)
                {
                    Page = converter.ForceArticle(url, html);
                }
                else
                {
                    Page = converter.ParseHtmlPage(url, html);
                }

                //========= Step 3: Render it
                if (Page == null)
                {
                    ErrorMessage = $"Could not parse HTML from '{url}'";
                    return false;
                }

                return true;

            } catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

    }
}

