using System;
using System.Diagnostics;

using NewsWaffle.Net;
using NewsWaffle.Converters;
using NewsWaffle.Models;
using AngleSharp.Dom;
using System.Data.SqlTypes;

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
        public AbstractPage GetPage(string urlString)
        {
            try
            {
                Uri url = LinkForge.Create(urlString);
                if(url == null)
                {
                    return null;
                }
                //========= Step 1: Get HTML
                var txt = GetContent(url);
                if(string.IsNullOrEmpty(txt))
                {
                    return null;
                }

                if(IsFeed(txt))
                {
                    var feed = FeedConverter.ParseFeed(url, txt);
                    feed.DownloadTime = (int)timer.ElapsedMilliseconds;
                    return feed;
                }

                WebConverter htmlConverter = new WebConverter(url, txt);

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

        public FeedPage GetFeedPage(string urlString)
        {
            Uri url = LinkForge.Create(urlString);
            if (url == null)
            {
                return null;
            }
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
        public LinkPage GetLinkPage(string urlString)
        {
            try
            {
                Uri url = LinkForge.Create(urlString);
                if (url == null)
                {
                    return null;
                }
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
        public ContentPage GetContentPage(string urlString)
        {
            try
            {
                Uri url = LinkForge.Create(urlString);
                if (url == null)
                {
                    return null;
                }
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
        public RawPage GetRawPage(string urlString)
        {
            try
            {
                Uri url = LinkForge.Create(urlString);
                if (url == null)
                {
                    return null;
                }
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

        private string GetContent(Uri url)
        {
            IHttpRequestor requestor = new HttpRequestor();
            timer.Start();
            var result = requestor.Request(url);
            timer.Stop();

            if (!result)
            {
                ErrorMessage = requestor.ErrorMessage;
                return null;
            }
            return requestor.BodyText;
        }

        private bool IsFeed(string content)
        {
            var prefix = content.Substring(0, Math.Min(content.Length, 250));
            return prefix.Contains("<rss") || prefix.Contains("<feed");
        }
    }
}

