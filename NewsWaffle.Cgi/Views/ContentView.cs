﻿using NewsWaffle.Models;

namespace NewsWaffle.Cgi.Views;

internal class ContentView : ArticleView
{
    ArticlePage ContentPage => (ArticlePage) Page;

    public ContentView(StreamWriter sw, ArticlePage page)
        : base(sw, page) { }

    protected override void Body()
    {
        Out.WriteLine();
        if (ContentPage.IsReadability)
        {
            Out.WriteLine(ContentPage.Content);
            if (ContentPage.Links.Count > 0)
            {
                Out.WriteLine();
                Out.WriteLine("## Article Links");
                foreach (var link in ContentPage.Links)
                {
                    var hostText = "";
                    if (link.Url.Scheme.StartsWith("http") && ContentPage.SourceUrl.Host != link.Url.Host)
                    {
                        hostText = $"({link.Url.Host}) ";
                    }
                    Out.WriteLine($"=> {LinkRewriter.GetLinkUrl(link.Url)} {link.OrderDetected}. {hostText}\"{link.Text}\"");
                }
            }
        }
        else
        {
            Out.WriteLine("Based on meta data, we thought this was a article, But we were unable to extract one. This might be a page of primarily links, like a home page or category page, or an oddly formatted page.");

            if (!string.IsNullOrEmpty(ContentPage.Excerpt))
            {
                Out.WriteLine("Excerpt:");
                Out.WriteLine($">{ContentPage.Excerpt}");
            }

            Out.WriteLine($"=> {CgiPaths.ViewRaw(ContentPage.Meta.SourceUrl)} Try in Raw View?");
            Out.WriteLine($"=> {CgiPaths.ViewLinks(ContentPage.Meta.SourceUrl)} Try in Link View?");
        }
    }

    protected override void Header()
    {
        if (ContentPage.Published != null)
        {
            Out.WriteLine($"Published: {ContentPage.Published.Value.ToString("yyyy-MM-dd HH:mm")} GMT");
        }
        if (!string.IsNullOrEmpty(ContentPage.Byline))
        {
            Out.WriteLine($"Byline: {ContentPage.Byline}");
        }
        if (ContentPage.WordCount > 0)
        {
            Out.WriteLine($"Length: {ContentPage.WordCount} words (~{ContentPage.TimeToRead.Minutes} minutes)");
        }
        if (ContentPage.Meta.FeaturedImage != null)
        {
            Out.WriteLine($"=> {LinkRewriter.GetImageUrl(ContentPage.Meta.FeaturedImage)} Featured Image");
        }
    }

    protected override void ReadOptions()
    {
        if (ContentPage.IsReadability)
        {
            Out.WriteLine($"=> {CgiPaths.ViewLinks(ContentPage.Meta.SourceUrl)} View in Link Mode");
        }
        Out.WriteLine($"=> {CgiPaths.ViewRaw(ContentPage.Meta.SourceUrl)} View in Raw View");
    }
}