using System;
using System.IO;
using Gemini.Cgi;

using NewsWaffle.Models;
namespace NewsWaffle.Cgi.Views
{
    internal abstract class ArticleView : AbstractView
    {
        protected AbstractPage Page;

        protected override IPageStats PageStats => Page;

        public ArticleView(StreamWriter sw, AbstractPage page)
            : base(sw)
        {
            Page = page;
        }

        protected override void RenderView()
        {
            RenderTitle(Page.Meta.SiteName);
            Out.WriteLine($"## {Page.Meta.Title}");
            Header();
            ReadOptions();
            Body();
        }

        protected abstract void Header();
        protected abstract void ReadOptions();
        protected abstract void Body();
    }
}