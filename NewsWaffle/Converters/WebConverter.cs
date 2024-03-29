﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using HtmlToGmi;
using NewsWaffle.Models;
using NewsWaffle.Util;
using SmartReader;

namespace NewsWaffle.Converters;

/// <summary>
/// Convers web pages into the appropriate page type
/// </summary>
public class WebConverter
{
    Uri Url;
    string Html;
    IHtmlDocument document;
    //represents the root element
    IElement documentRoot;
    PageMetaData MetaData;
    Stopwatch timer = new Stopwatch();

    public WebConverter(Uri url, string html)
    {
        Url = url;
        Html = html;
    }

    #region public methods

    /// <summary>
    /// Convert the page, auto-detecting the type
    /// </summary>
    public AbstractPage Convert()
    {
        timer.Start();
        EnsureParsed();
        switch (MetaData.ProbablyType)
        {
            case PageType.ArticlePage:
                return ConvertToContentPage();

            default:
                return ConvertToLinkPage();
        }
    }

    /// <summary>
    /// Convert the Link Page
    /// </summary>
    public LinkPage ConvertToLinkPage()
    {
        if (!timer.IsRunning)
        {
            timer.Start();
        }
        EnsureParsed();
        LinkParser extractor = new LinkParser(Url);
        extractor.FindLinks(documentRoot);
        var homePage = new LinkPage(MetaData)
        {
            ArticleLinks = extractor.ContentLinks,
            NavigationLinks = extractor.NavigationLinks,
            FeedUrl = extractor.FeedUrl,
        };
        timer.Stop();
        homePage.ConvertTime = (int)timer.ElapsedMilliseconds;
        return homePage;
    }

    /// <summary>
    /// Convert to Content Page
    /// </summary>
    /// <returns></returns>
    public ArticlePage ConvertToContentPage()
    {
        if (!timer.IsRunning)
        {
            timer.Start();
        }
        EnsureParsed();

        Article article = null;
        try
        {
            //handle exceptions that do happen down in the parser that aren't caught
            //like this fix: https://github.com/Strumenta/SmartReader/commit/7053e67c0ef00047e645e12deba91d4144f0392d
            var reader = new Reader(Url.AbsoluteUri, document);
            article = reader.GetArticle();
        }
        catch (Exception ex)
        {
        }
        ArticlePage page = null;

        if (article != null && article.IsReadable && article.Content != "")
        {
            var converter = new HtmlToGmi.HtmlConverter
            {
                ShouldRenderHyperlinks = false,
                ImageRewriteCallback = LinkRewriter.GetImageUrl
            };
            var result = converter.Convert(Url, ParseToDocument(article.Content));

            page = new ArticlePage(MetaData)
            {
                IsReadability = true,
                Byline = article.Author ?? article.Byline,
                Content = result.Gemtext,
                Images = result.Images.ToList(),
                Links = result.Links.ToList(),
                Published = article.PublicationDate,
                TimeToRead = article.TimeToRead,
                WordCount = CountWords(result.Gemtext),
            };

            page.Meta.Title = StringUtils.Normnalize(article.Title);
        }
        else
        {
            page = new ArticlePage(MetaData)
            {
                IsReadability = false,
                Excerpt = FindBestExcerpt(MetaData, article)
            };
        }
        timer.Stop();
        page.ConvertTime = (int)timer.ElapsedMilliseconds;
        return page;
    }

    /// <summary>
    /// Convert to Content Page
    /// </summary>
    /// <returns></returns>
    public RawPage ConvertToRawPage()
    {
        if (!timer.IsRunning)
        {
            timer.Start();
        }
        EnsureParsed();

        TagStripper.RemoveNavigation(documentRoot);

        var converter = new HtmlToGmi.HtmlConverter
        {
            ShouldRenderHyperlinks = true,
            ImageRewriteCallback = LinkRewriter.GetImageUrl,
            AnchorRewriteCallback = LinkRewriter.GetLinkUrl
        };
        var result = converter.Convert(Url, document);

        var page = new RawPage(MetaData)
        {
            Content = result.Gemtext,
            Links = result.Links.ToList(),
        };
        timer.Stop();
        page.ConvertTime = (int)timer.ElapsedMilliseconds;
        return page;
    }

    #endregion

    #region private workings

    private string FindBestExcerpt(PageMetaData metaData, Article? article)
    {
        var meta = metaData.Description ?? "";
        var excerpt = StringUtils.Normnalize(article?.Excerpt);
        return (meta.Length > excerpt.Length) ?
            meta : excerpt;
    }

    private void EnsureParsed()
    {
        if (document == null)
        {
            document = ParseToDocument(Html);
        }
        if (documentRoot == null)
        {
            documentRoot = document.FirstElementChild;
        }
        if (MetaData == null)
        {
            ParseMetadata();
        }
    }

    private int CountWords(string content)
        => content.Split("\n").Where(x => !x.StartsWith("=> ")).Sum(x => CountWordsInLine(x));

    private int CountWordsInLine(string s)
        => s.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;


    private void ParseMetadata()
    {
        MetaDataParser parser = new MetaDataParser();
        MetaData = parser.GetMetaData(Url, Html, documentRoot);
    }

    private void SaveHtml(string filename, string html)
        => File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/tmp/" + filename, html);

    private IHtmlDocument ParseToDocument(string html)
    {
        var context = BrowsingContext.New(Configuration.Default);
        var parser = context.GetService<IHtmlParser>();
        return parser.ParseDocument(html);
    }
    #endregion
}
