using System;

using NewsWaffle.Net;
using NewsWaffle.Converter;
using NewsWaffle.Models;

namespace NewsWaffle
{
    public class YummyWaffles
    {

        public bool Debug { get; set; } = true;
        public string ErrorMessage { get; internal set; } = "";

        /// <summary>
        /// Gets a page, auto-detect page type, and return it
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public AbstractPage GetPage(string url)
        {
            try
            {
                //========= Step 1: Get HTML
                var html = GetHtml(url);
                if(string.IsNullOrEmpty(html))
                {
                    return null;
                }

                HtmlConverter htmlConverter = new HtmlConverter(url, html);

                //convert, autodetecting
                var page = htmlConverter.Convert();

                if (page == null)
                {
                    ErrorMessage = $"Could not parse HTML from '{url}'";
                    return null;
                }

                return page;

            } catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Gets a page, force parsing it as a LinkPage
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public LinkPage GetLinkPage(string url)
        {
            try
            {
                //========= Step 1: Get HTML
                var html = GetHtml(url);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }

                HtmlConverter htmlConverter = new HtmlConverter(url, html);

                //convert, autodetecting
                var page = htmlConverter.ConvertToLinkPage();

                if (page == null)
                {
                    ErrorMessage = $"Could not parse HTML from '{url}'";
                    return null;
                }

                return page;

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Gets a page, force parsing it as a LinkPage
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ContentPage GetContentPage(string url)
        {
            try
            {
                //========= Step 1: Get HTML
                var html = GetHtml(url);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }

                HtmlConverter htmlConverter = new HtmlConverter(url, html);

                //convert, autodetecting
                var page = htmlConverter.ConvertToContentPage();

                if (page == null)
                {
                    ErrorMessage = $"Could not parse HTML from '{url}'";
                    return null;
                }

                return page;

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }


        private string GetHtml(string url)
        {
            var fetcher = new HttpFetcher();
            var html = fetcher.GetAsString(url);
            if (string.IsNullOrEmpty(html))
            {
                ErrorMessage = $"Could not download HTML for '{url}'";
                return null;
            }
            return html;
        }


    }
}

