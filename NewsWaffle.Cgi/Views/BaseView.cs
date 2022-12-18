using System;
using Gemini.Cgi;
using NewsWaffle.Models;

namespace NewsWaffle.Cgi.Views
{
	internal class BaseView
	{

        protected StreamWriter Out;

        public BaseView(StreamWriter sw)
        {
            Out = sw;
        }

        protected void RenderTitle(string subTitle = "")
        {
            if (string.IsNullOrEmpty(subTitle))
            {
                Out.WriteLine($"# 🧇 NewsWaffle");
            }
            else
            {
                Out.WriteLine($"# 🧇 NewsWaffle: {subTitle}");
            }
        }

        protected void RenderFooter(IPageStats page = null)
        {
            Out.WriteLine();
            if (!string.IsNullOrEmpty(page?.Copyright ?? null))
            {
                Out.WriteLine($"All content © {DateTime.Now.Year} {page.Copyright}");
            }
            Out.WriteLine("---");
            if (page != null)
            {
                Out.WriteLine($"Size: {ReadableFileSize(page.Size)}. {Savings(page.Size, page.OriginalSize)} smaller than original: {ReadableFileSize(page.OriginalSize)} 🤮");
                Out.WriteLine($"Fetched: {page.DownloadTime} ms. Converted: {page.ConvertTime} ms 🐇");
                Out.WriteLine($"=> {page.SourceUrl} Link to Source");
                Out.WriteLine("---");
            }
            Out.WriteLine("=> mailto:acidus@gemi.dev Made with 🧇 and ❤️ by Acidus");
        }

        private string Savings(int newSize, int originalSize)
            => string.Format("{0:0.00}%", (1.0d - (Convert.ToDouble(newSize) / Convert.ToDouble(originalSize))) * 100.0d);

        private string ReadableFileSize(double size, int unit = 0)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            while (size >= 1024)
            {
                size /= 1024;
                ++unit;
            }

            return string.Format("{0:0.0#} {1}", size, units[unit]);
        }
    }
}
