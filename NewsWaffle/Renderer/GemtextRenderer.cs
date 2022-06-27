using System;
using System.IO;

using NewsWaffle.Models;

namespace NewsWaffle.Renderer
{
    public class GemtextRenderer
    {
        ArticlePage Page;

        public GemtextRenderer(ArticlePage page)
        {
            Page = page;
        }

        public void WriteToFile(string outFile)
        {
            StreamWriter fout = new StreamWriter(outFile);

            fout.WriteLine(">Converted by GemiWebProxy");
            fout.WriteLine();

            fout.WriteLine($"# {Page.Title}");
            if(Page.FeaturedImage != null)
            {
                fout.WriteLine($"=> {Page.FeaturedImage} Featured Image");
            }

            foreach(var item in Page.Content)
            {   
                fout.Write(item.Content);
            }
            fout.Close();
        }
    }
}