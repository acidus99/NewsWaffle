using System;
using System.Diagnostics;

using NewsWaffle.Net;
using NewsWaffle.Converters;
using NewsWaffle.Models;

namespace NewsWaffle
{
    /// <summary>
    /// Fetches content over HTTP and converts it to different news formats
    /// </summary>
    public class NewsConverter
    {

        public bool Debug { get; set; } = true;
        public string ErrorMessage { get; internal set; } = "";

        Stopwatch timer = new Stopwatch();

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
                var html = GetContent(url);
                if(string.IsNullOrEmpty(html))
                {
                    return null;
                }

                if(IsFeed(html))
                {
                    var feed = FeedConverter.ParseFeed(url, html);
                    feed.DownloadTime = (int)timer.ElapsedMilliseconds;
                    return feed;
                }

                WebConverter htmlConverter = new WebConverter(url, html);

                //convert, autodetecting
                var page = htmlConverter.Convert();

                if (page == null)
                {
                    ErrorMessage = $"Could not parse HTML from '{url}'";
                    return null;
                }
                page.DownloadTime = (int)timer.ElapsedMilliseconds;
                return page;

            } catch(Exception ex)
            {
                ErrorMessage = ex.Message + ex.StackTrace;
                return null;
            }
        }

        public FeedPage GetFeedPage(string url)
        {
            var xml = GetContent(url);
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            var page = FeedConverter.ParseFeed(url, xml);
            if(page == null)
            {
                ErrorMessage = $"Could not parse RSS/Atom feed from '{url}'";
                return null;
            }
            page.DownloadTime = (int)timer.ElapsedMilliseconds;
            return page;

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
                var html = GetContent(url);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }

                WebConverter htmlConverter = new WebConverter(url, html);

                //convert, autodetecting
                var page = htmlConverter.ConvertToLinkPage();

                if (page == null)
                {
                    ErrorMessage = $"Could not parse HTML from '{url}'";
                    return null;
                }
                page.DownloadTime = (int)timer.ElapsedMilliseconds;
                return page;

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message + ex.StackTrace;
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
                var html = GetContent(url);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }

                WebConverter htmlConverter = new WebConverter(url, html);

                //convert, autodetecting
                var page = htmlConverter.ConvertToContentPage();

                if (page == null)
                {
                    ErrorMessage = $"Could not parse HTML from '{url}'";
                    return null;
                }
                page.DownloadTime = (int)timer.ElapsedMilliseconds;
                return page;

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message + ex.StackTrace;
                return null;
            }
        }

        /// <summary>
        /// Gets a page, using Raw mode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public RawPage GetRawPage(string url)
        {
            try
            {
                //========= Step 1: Get HTML
                var html = GetContent(url);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }

                var htmlConverter = new WebConverter(url, html);
                var page = htmlConverter.ConvertToRawPage();

                if (page == null)
                {
                    ErrorMessage = $"Could not parse HTML from '{url}'";
                    return null;
                }
                page.DownloadTime = (int)timer.ElapsedMilliseconds;
                return page;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message + ex.StackTrace;
                return null;
            }
        }

        private string GetContent(string url)
        {
            timer.Start();
            var fetcher = new HttpFetcher();
            var html = fetcher.GetAsString(url);
            timer.Stop();
            if (string.IsNullOrEmpty(html))
            {
                ErrorMessage = $"Could not download HTML for '{url}'";
                return null;
            }
            return html;
        }

        private bool IsFeed(string content)
        {
            var prefix = content.Substring(0, Math.Min(content.Length, 250));
            return prefix.Contains("<rss") || prefix.Contains("<feed");
        }
    }
}

