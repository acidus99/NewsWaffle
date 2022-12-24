using System.Linq;
using System.Collections.Generic;

using HtmlToGmi.Models;

namespace NewsWaffle.Models
{
	public class RawPage :AbstractPage
	{
		public RawPage(PageMetaData metaData)
			: base(metaData) {}

		public string Content = "";
		public List<Hyperlink> Links = new List<Hyperlink>();

		public override int Size
			=> Content.Length;
	}
}

