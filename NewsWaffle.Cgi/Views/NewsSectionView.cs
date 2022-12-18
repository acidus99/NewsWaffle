using System;
using Gemini.Cgi;

using NewsWaffle.Aggregators;
using NewsWaffle.Aggregators.Models;
using NewsWaffle.Models;

namespace NewsWaffle.Cgi.Views
{
	internal class NewsSectionView : AbstractView
    {
        NewsSection NewsSection;

        protected override IPageStats PageStats => NewsSection;

        public NewsSectionView(StreamWriter sw, NewsSection newsSection)
            : base(sw)
        {
            NewsSection = newsSection;
        }

        protected override void RenderView()
        {
            RenderTitle("Current News");
            Out.WriteLine($"=> {CgiPaths.SwitchNewsSection} Current Section: {NewsSection.SectionName}. Change?");
            Out.WriteLine();
            if (NewsSection.Stories.Count > 0)
            {
                int counter = 0;
                foreach (var story in NewsSection.Stories.OrderByDescending(x => x.Updated))
                {
                    counter++;
                    Out.WriteLine($"=> {CgiPaths.ViewArticle(story.Url)} {counter}. {story.Title} ({story.Source}, {story.TimeAgo})");
                }
            }
            else
            {
                Out.WriteLine("This sections doesn't have any news stories"); ;
            }
        }
    }
}