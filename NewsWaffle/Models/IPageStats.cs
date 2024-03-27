using System;

namespace NewsWaffle.Models;

public interface IPageStats
{
    //time to download the page, in ms
    int DownloadTime { get; }

    //time to convert the page, in ms
    int ConvertTime { get; }

    //original size of the page
    int OriginalSize { get; }

    //copyright string
    string Copyright { get; }

    //size of the optimized gemini page
    int Size { get; }

    Uri SourceUrl { get; }

}
