using System;

using AngleSharp.Html.Dom;
using AngleSharp.Dom;

using NewsWaffle.Models;

namespace NewsWaffle.Converter.Special
{
    public static class MediaConverter
    {
        public static MediaItem ConvertImg(HtmlElement img)
        {
            var url = GetUrl(img);
            if(url == null)
            {
                return null;
            }
            return new MediaItem
            {
                Caption = GetCaption(img),
                Url = url
            };
        }

        private static string GetCaption(HtmlElement img)
        {
            var caption = img.GetAttribute("alt") ?? "";
            caption = caption.Trim();
            return caption.Length > 0 ? caption : "Article Image";
        }

        private static string GetUrl(HtmlElement img)
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

