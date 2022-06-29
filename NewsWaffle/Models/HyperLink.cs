using System;
namespace NewsWaffle.Models
{
    public class HyperLink
    {
        public int OrderDetected { get; set; }
        public string Text { get; set; }
        public Uri Url { get; set; }

        //size of a text representation of this link
        public int Size
            => Url.AbsoluteUri.Length + Text.Length + 5;
    }
}

