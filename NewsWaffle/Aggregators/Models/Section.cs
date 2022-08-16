using System;
using System.Collections.Generic;
namespace NewsWaffle.Aggregators.Models
{
	public class Section
	{
		public string AggregatorName { get; private set; }

		public String SectionName { get; private set; }

		public List<NewsStory> Stories { get; private set; }

		internal Section(string sectionName, string aggregatorName)
        {
			SectionName = sectionName;
			AggregatorName = aggregatorName;
			Stories = new List<NewsStory>();
        }
	}
}

