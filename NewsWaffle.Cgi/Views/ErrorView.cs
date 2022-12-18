using System;
using System.IO;
using Gemini.Cgi;

using NewsWaffle.Models;
namespace NewsWaffle.Cgi.Views
{
    internal class ErrorView : BaseView
    {
        public ErrorView(StreamWriter sw)
            : base(sw) { }

        public void RenderError(string msg)
        {
            Out.WriteLine($"# 🧇 NewsWaffle");
            Out.WriteLine("Bummer dude! Error Wafflizing that page.");
            Out.WriteLine(msg);
            RenderFooter();
        }
    }
}
