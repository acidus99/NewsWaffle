using System;
using System.IO;

using NewsWaffle.Models;

namespace NewsWaffle.Renderer
{
    public class GemtextRenderer
    {
        ContentPage Page;

        public GemtextRenderer(ContentPage page)
        {
            Page = page;
        }

        public void WriteToFile(string outFile)
        {
            StreamWriter fout = new StreamWriter(outFile);

            fout.WriteLine(">Converted by GemiWebProxy");
            fout.WriteLine();

            fout.WriteLine($"# {Page.Meta.Title}");
            if(Page.Meta.FeaturedImage != null)
            {
                fout.WriteLine($"=> {Page.Meta.FeaturedImage} Featured Image");
            }

            foreach(var item in Page.Content)
            {   
                fout.Write(item.Content);
            }
            fout.Close();
        }
    }
}