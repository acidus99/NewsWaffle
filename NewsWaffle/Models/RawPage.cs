using System.Linq;
using System.Collections.Generic;
namespace NewsWaffle.Models
{
	public class RawPage :AbstractPage
	{
		public RawPage(PageMetaData metaData)
			: base(metaData) {}

		public List<ContentItem> Content = new List<ContentItem>();
		public LinkCollection Links = new LinkCollection();

		public override int Size
			=> Content.Sum(x => x.Content.Length);
	}
}

