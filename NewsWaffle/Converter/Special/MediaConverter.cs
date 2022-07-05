using System;

using AngleSharp.Html.Dom;
using AngleSharp.Dom;

using NewsWaffle.Models;

namespace NewsWaffle.Converter.Special
{
    public static class MediaConverter
    {
        /// <summary>
        /// Attempts to convert an IMG tag into a MediaItem using
        /// the IMG ALT attribute as the caption
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static MediaItem ConvertImg(HtmlElement img)
        {
            var url = GetUrl(img);
            if(url == null)
            {
                return null;
            }
            return new MediaItem
            {
                Caption = GetAlt(img),
                Url = url
            };
        }

        /// <summary>
        /// Attempts to convert a FIGURE tag into a MediaItem, using
        /// the IMG tag for the image, and the FIGCAPTION tag, or the IMG ALT
        /// as the caption
        /// </summary>
        /// <param name="figure"></param>
        /// <returns></returns>
        public static MediaItem ConvertFigure(HtmlElement figure)
        {
            var img = figure.QuerySelector("img");
            //can't find an image? Nothing we can do
            if(img == null)
            {
                return null;
            }

            var url = GetUrl(img);
            //no link? nothing I can do
            if (url == null)
            {
                return null;
            }

            return new MediaItem
            {
                Caption = FindCaption(figure, img),
                Url = url
            };
        }

        private static string FindCaption(IElement figure, IElement img)
        {
            var ret = GetFigCaption(figure);
            return (ret.Length > 0) ? ret : GetAlt(img);
        }

        private static string GetAlt(IElement img)
        {
            var caption = img.GetAttribute("alt") ?? "";
            caption = caption.Trim();
            return caption.Length > 0 ? caption : "Article Image";
        }

        private static string GetFigCaption(IElement figure)
            => NewlineStripper.RemoveNewlines(figure.QuerySelector("figcaption")?.TextContent ?? "");

        private static string GetUrl(IElement img)
        {
            //older sits using non-native lazy loading will have source as a data-src attribute
            var url = img.GetAttribute("data-src");
            if(string.IsNullOrEmpty(url))
            {
                url = img.GetAttribute("src");
            }
            return url;
        }
    }
}