using System;

using NewsWaffle.Aggregators.Models;

namespace NewsWaffle.Aggregators
{
	public interface INewAggregator
	{
		/// <summary>
        /// The name of the news aggregator
        /// </summary>
		public string Name { get; }

        public string DefaultSection { get; }

        public string [] AvailableSections { get; }

		/// <summary>
        /// Returns the web resource to request for a section
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
		public string GetFeedUrl(string sectionName);

		/// <summary>
        /// parses the web resource for a section
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
		public Section ParseSection(string sectionName, string content);
	}
}

