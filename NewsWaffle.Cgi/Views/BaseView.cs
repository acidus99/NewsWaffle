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
                Out.WriteLine($"Size: {RenderUtils.ReadableFileSize(page.Size)}. {RenderUtils.Savings(page.Size, page.OriginalSize)} smaller than original: {RenderUtils.ReadableFileSize(page.OriginalSize)} 🤮");
                Out.WriteLine($"Fetched: {page.DownloadTime} ms. Converted: {page.ConvertTime} ms 🐇");
                Out.WriteLine($"=> {page.SourceUrl} Link to Source");
                Out.WriteLine("---");
            }
            Out.WriteLine("=> mailto:acidus@gemi.dev Made with 🧇 and ❤️ by Acidus");
        }

    }
}

