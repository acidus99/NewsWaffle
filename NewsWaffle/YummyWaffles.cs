using System;

using NewsWaffle.Net;
using NewsWaffle.Converter;
using NewsWaffle.Models;

namespace NewsWaffle
{
    public class YummyWaffles
    {

        public bool Debug { get; set; } = true;

        public AbstractPage Page { get; internal set; } = null;

        public string ErrorMessage { get; internal set; } = "";

        public bool GetPage(string url, bool forceArticle = false)
        {
            try
            {
                //========= Step 1: Get HTML
                var fetcher = new HttpFetcher();
                var html = fetcher.GetAsString(url);

                if(Debug)
                {
                    Save("original.html", html);
                }

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
                if(Debug && Page is ArticlePage)
                {
                    Save("simplified.html", ((ArticlePage)Page).SimplifiedHtml);
                }

                return true;

            } catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

        private void Save(string filename, string html)
            => System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/tmp/" + filename, html);


    }
}

