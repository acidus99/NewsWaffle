using System;
namespace NewsWaffle.Converter.Filter
{
	/// <summary>
    /// represents a rule of DOM nodes we want to filter
    /// </summary>
	internal class FilterRule
	{
		public string ClassName { get; set; } = null;

		public string ID { get; set; } = null;

		public string TagName { get; set; } = null;

		public bool HasClass
			=> !String.IsNullOrEmpty(ClassName);

		public bool HasID
			=> !String.IsNullOrEmpty(ID);

		public bool HasTag
			=> !String.IsNullOrEmpty(TagName);
	}
}

