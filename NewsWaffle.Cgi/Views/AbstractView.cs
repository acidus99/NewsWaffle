using NewsWaffle.Models;

namespace NewsWaffle.Cgi.Views;

internal abstract class AbstractView
{

    protected StreamWriter Out;

    abstract protected IPageStats? PageStats { get; }

    public AbstractView(StreamWriter sw)
    {
        Out = sw;
    }

    public void Render()
    {
        RenderView();
        RenderFooter();
    }

    protected abstract void RenderView();

    protected void RenderTitle(string subTitle = "")
    {
        if (string.IsNullOrEmpty(subTitle))
        {
            Out.WriteLine($"# 🧇 NewsWaffle");
        }
        else
        {
            Out.WriteLine($"# 🧇 NewsWaffle: {subTitle}");
        }
    }

    private void RenderFooter()
    {
        Out.WriteLine();
        if (PageStats != null && PageStats.Copyright.Length > 0)
        {
            Out.WriteLine($"All content © {DateTime.Now.Year} {PageStats.Copyright}");
        }
        Out.WriteLine("---");
        if (PageStats != null)
        {
            Out.WriteLine($"Size: {ReadableFileSize(PageStats.Size)}. {Savings(PageStats.Size, PageStats.OriginalSize)} smaller than original: {ReadableFileSize(PageStats.OriginalSize)} 🤮");
            Out.WriteLine($"Fetched: {PageStats.DownloadTime} ms. Converted: {PageStats.ConvertTime} ms 🐇");
            Out.WriteLine($"=> {PageStats.SourceUrl} Link to Source");
            Out.WriteLine("---");
        }
        Out.WriteLine("=> mailto:acidus@gemi.dev Made with 🧇 and ❤️ by Acidus");
    }

    private string Savings(int newSize, int originalSize)
        => string.Format("{0:0.00}%", (1.0d - (Convert.ToDouble(newSize) / Convert.ToDouble(originalSize))) * 100.0d);

    private string ReadableFileSize(double size, int unit = 0)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        while (size >= 1024)
        {
            size /= 1024;
            ++unit;
        }

        return string.Format("{0:0.0#} {1}", size, units[unit]);
    }
}
