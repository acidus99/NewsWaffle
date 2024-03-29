﻿using System.Collections.Generic;
using HtmlToGmi.Models;

namespace NewsWaffle.Models;

public class RawPage : AbstractPage
{

    public string Content = "";

    public List<Hyperlink> Links = new List<Hyperlink>();

    public override int Size
        => Content.Length;

    public RawPage(PageMetaData metaData)
        : base(metaData) { }
}
