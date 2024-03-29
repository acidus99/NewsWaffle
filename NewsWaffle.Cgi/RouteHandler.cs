﻿using Gemini.Cgi;
using NewsWaffle.Aggregators;
using NewsWaffle.Cgi.Media;
using NewsWaffle.Cgi.Views;
using NewsWaffle.Models;

namespace NewsWaffle.Cgi;

public static class RouteHandler
{
    public static void View(CgiWrapper cgi)
    {
        if (!cgi.HasQuery)
        {
            cgi.Input("Enter URL of news site (e.g. 'https://www.wired.com'");
            return;
        }
        cgi.Success();

        var waffles = new LegacyNewsConverter();
        var page = waffles.GetPage(cgi.Query);

        AbstractView view;
        if (page == null)
        {
            view = new ErrorView(cgi.Writer, waffles.ErrorMessage);
        }
        else if (page is LinkPage)
        {
            view = new LinksView(cgi.Writer, (LinkPage)page);
        }
        else if (page is ArticlePage)
        {
            view = new ContentView(cgi.Writer, (ArticlePage)page);
        }
        else
        {
            view = new FeedView(cgi.Writer, (FeedPage)page);
        }
        view.Render();
    }

    public static void Article(CgiWrapper cgi)
    {
        if (!cgi.HasQuery)
        {
            cgi.Input("Enter URL of news site (e.g. 'https://www.wired.com'");
            return;
        }
        cgi.Success();
        var waffles = new LegacyNewsConverter();
        var page = waffles.GetContentPage(cgi.Query);

        AbstractView view = (page == null) ?
            new ErrorView(cgi.Writer, waffles.ErrorMessage) :
            new ContentView(cgi.Writer, (ArticlePage)page);

        view.Render();
    }

    public static void Links(CgiWrapper cgi)
    {
        if (!cgi.HasQuery)
        {
            cgi.Input("Enter URL of news site (e.g. 'https://www.wired.com'");
            return;
        }
        cgi.Success();
        var waffles = new LegacyNewsConverter();
        var linksPage = waffles.GetLinkPage(cgi.Query);

        AbstractView view = (linksPage == null) ?
            new ErrorView(cgi.Writer, waffles.ErrorMessage) :
            new LinksView(cgi.Writer, linksPage);

        view.Render();
    }

    public static void Feed(CgiWrapper cgi)
    {
        if (!cgi.HasQuery)
        {
            cgi.Redirect(CgiPaths.BasePath);
            return;
        }
        cgi.Success();

        var waffles = new LegacyNewsConverter();
        var feedPage = waffles.GetFeedPage(cgi.Query);
        AbstractView view = (feedPage == null) ?
            new ErrorView(cgi.Writer, waffles.ErrorMessage) :
            new FeedView(cgi.Writer, feedPage);

        view.Render();
    }

    public static void Raw(CgiWrapper cgi)
    {
        if (!cgi.HasQuery)
        {
            cgi.Redirect(CgiPaths.BasePath);
            return;
        }
        cgi.Success();

        var waffles = new LegacyNewsConverter();
        var rawPage = waffles.GetRawPage(cgi.Query);

        AbstractView view = (rawPage == null) ?
            new ErrorView(cgi.Writer, waffles.ErrorMessage) :
            new RawView(cgi.Writer, rawPage);

        view.Render();
    }

    public static void CurrentNews(CgiWrapper cgi)
    {
        var yahooNews = new YahooNews();

        var sectionName = cgi.HasQuery ? cgi.SantiziedQuery : yahooNews.DefaultSection;
        if (!yahooNews.IsValidSection(sectionName))
        {
            cgi.Redirect(CgiPaths.ViewCurrentNewsSection());
            return;
        }

        cgi.Success();
        var newsSection = AggregatorFetcher.GetSection(sectionName, yahooNews);
        AbstractView view = (newsSection == null) ?
            new ErrorView(cgi.Writer, "Couldn't fetch current news") :
            new NewsSectionView(cgi.Writer, newsSection);

        view.Render();
    }

    public static void SwitchNewsSection(CgiWrapper cgi)
    {
        cgi.Success();
        var view = new SectionSelectView(cgi.Writer, new YahooNews());
        view.Render();
    }

    public static void ProxyMedia(CgiWrapper cgi)
    {
        if (!cgi.HasQuery)
        {
            cgi.Redirect(CgiPaths.BasePath);
            return;
        }
        MediaProxy proxy = new MediaProxy();
        byte[]? optimizedImage = proxy.ProxyMedia(cgi.Query);
        if (optimizedImage == null)
        {
            cgi.BadRequest("Couldn't fetch media");
            return;
        }
        cgi.Success("image/jpeg");
        cgi.Out.Write(optimizedImage);
    }

}