using NewsWaffle.Models;

namespace NewsWaffle.Cgi.Views;

internal class FeedView : ArticleView
{
    private FeedPage FeedPage => (FeedPage)Page;

    public FeedView(StreamWriter sw, FeedPage page)
        : base(sw, page) { }

    protected override void Body()
    {
        Out.WriteLine();
        Out.WriteLine($"### Feed Items: {FeedPage.Items.Count}");
        if (FeedPage.Items.Count > 0)
        {
            int counter = 0;
            foreach (var item in FeedPage.Items)
            {
                counter++;
                Out.WriteLine($"=> {CgiPaths.ViewArticle(item.Url)} {counter}. {item.Title} {item.GetTimeAgo(DateTime.Now)}");
            }
        }
        else
        {
            Out.WriteLine("This feed doesn't actually have any items in it. Unfortunately some sites have broken feeds with content.");
        }
    }

    protected override void Header()
    {
        if (FeedPage.Meta.FeaturedImage != null)
        {
            Out.WriteLine($"=> {LinkRewriter.GetImageUrl(FeedPage.Meta.FeaturedImage)} Featured Image");
        }
        if (FeedPage.Meta.Description.Length > 0)
        {
            Out.WriteLine($">{FeedPage.Meta.Description}");
        }
    }

    protected override void ReadOptions()
    {
        if (FeedPage.RootUrl != null)
        {
            Out.WriteLine($"=> {CgiPaths.ViewLinks(FeedPage.RootUrl)} Mode: RSS View. Try Link View?");
        }
    }
}