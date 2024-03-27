using NewsWaffle.Models;

namespace NewsWaffle.Cgi.Views;

internal class ErrorView : AbstractView
{
    string ErrorMsg;

    protected override IPageStats? PageStats => null;

    public ErrorView(StreamWriter sw, string msg)
        : base(sw)
    {
        ErrorMsg = msg;
    }

    protected override void RenderView()
    {
        RenderTitle();
        Out.WriteLine("Bummer dude! Error Wafflizing that page.");
        Out.WriteLine(ErrorMsg);
    }
}
