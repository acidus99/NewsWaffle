using System;
using Gemini.Cgi;

using NewsWaffle.Aggregators;
using NewsWaffle.Aggregators.Models;

namespace NewsWaffle.Cgi.Views
{
	internal class AggregatorView : BaseView
    {
        public AggregatorView(StreamWriter sw)
            : base(sw) { }

        public void RenderNewsSection(NewsSection section)
        {
            RenderTitle("Current News");
            Out.WriteLine($"=> {CgiPaths.SwitchNewsSection} Current Section: {section.SectionName}. Change?");
            Out.WriteLine();
            if (section.Stories.Count > 0)
            {
                int counter = 0;
                foreach (var story in section.Stories.OrderByDescending(x => x.Updated))
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

        public void RenderSectionOptions(INewAggregator aggregator)
        {
            RenderTitle("Current News");
            Out.WriteLine("Choose a section:");
            foreach (var section in aggregator.AvailableSections)
            {
                Out.WriteLine($"=> {CgiPaths.ViewCurrentNewsSection(section)} {section}");
            }
        }
    }
}

