using System;
using Gemini.Cgi;

using NewsWaffle.Aggregators;
using NewsWaffle.Aggregators.Models;
using NewsWaffle.Models;

namespace NewsWaffle.Cgi.Views
{
	internal class SectionSelectView : AbstractView
    {
        INewAggregator Aggregator;

        public SectionSelectView(StreamWriter sw, INewAggregator aggregator)
            : base(sw)
        {
            Aggregator = aggregator;
        }

        protected override IPageStats PageStats => null;

        protected override void RenderView()
        {
            RenderTitle("Current News");
            Out.WriteLine($"Aggregator: {Aggregator.Name}");
            Out.WriteLine("Choose a section:");
            foreach (var section in Aggregator.AvailableSections)
            {
                Out.WriteLine($"=> {CgiPaths.ViewCurrentNewsSection(section)} {section}");
            }
        }
    }
}