﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace NewsWaffle.Models
{
    public class ArticlePage : AbstractPage
    {
        public int WordCount { get; internal set; }
        public TimeSpan TimeToRead { get; internal set; }

        //content and images
        public List<ContentItem> Content = new List<ContentItem>();

        public List<MediaItem> Images = new List<MediaItem>();

        public int Size
            => Content.Sum(x => x.Content.Length);

        //For debugging
        public string SimplifiedHtml { get; set; }

    }
}
