using System;
using System.IO;
using Gemini.Cgi;

using NewsWaffle.Models;
namespace NewsWaffle.Cgi.Views
{
    internal class RawView : ArticleView
    {
        RawPage RawPage => (RawPage) Page;

        public RawView(StreamWriter sw, RawPage page)
            : base(sw, page) { }

        protected override void Body()
        {
            Out.WriteLine();
            foreach (var item in RawPage.Content)
            {
                Out.Write(item.Content);
            }
        }

        protected override void Header()
        {
            if (RawPage.Meta.FeaturedImage != null)
            {
                Out.WriteLine($"=> {MediaRewriter.GetPath(RawPage.Meta.FeaturedImage)} Featured Image");
            }
        }

        protected override void ReadOptions()
        {
            Out.WriteLine($"=> {CgiPaths.ViewArticle(RawPage.Meta.SourceUrl)} Mode: Raw View. Try in Article View?");
        }
    }
}
