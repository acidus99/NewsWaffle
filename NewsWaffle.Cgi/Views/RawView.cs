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
            if (RawPage.Content == "")
            {
                if (!string.IsNullOrEmpty(RawPage.Meta.Description))
                {
                    Out.WriteLine("Description:");
                    Out.WriteLine($">{RawPage.Meta.Description}");
                }

                Out.WriteLine(@"
When we converted the HTML to gemtext, there was no content left. This could be:
* An oddly formated page
* A page that requires JavaScript to render
* Content which is protected in some way
* A site that is not sending full content to certin User-Agents

Unfortunately there isn't really anything more we can do.");
            }
            else
            {
                Out.WriteLine();
                Out.WriteLine(RawPage.Content);
            }
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
