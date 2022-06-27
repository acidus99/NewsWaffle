using System;
namespace NewsWaffle.Models
{
    public class ContentItem : SectionItem
    {
        public string Content { get; set; }

        public bool HasContent
            => (Content.Trim().Length > 0);

        public ContentItem() { }
    }
}

