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
            Out.WriteLine(RawPage.Content);
        }

        protected override void Header()
        {
            if (RawPage.Meta.FeaturedImage != null)
            {
                Out.WriteLine($"=> {LinkRewriter.GetImageUrl(RawPage.Meta.FeaturedImage)} Featured Image");
            }
        }

        protected override void ReadOptions()
        {
            Out.WriteLine($"=> {CgiPaths.ViewArticle(RawPage.Meta.SourceUrl)} Mode: Raw View. Try in Article View?");
        }
    }
}
