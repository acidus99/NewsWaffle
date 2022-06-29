using System;

using ImageMagick;

namespace NewsWaffle.Cgi.Media
{
    /// <summary>
    /// Reformats media from Wikipedia to better suit Gemini clients
    /// </summary>
    public static class MediaProcessor
    {
        public static byte [] ProcessImage(byte[] data)
        {
            using (var image = new MagickImage(data))
            {
                if (!image.IsOpaque)
                {
                    //add a white background to transparent images to
                    //make them visible on clients with a dark theme
                    image.BackgroundColor = new MagickColor("white");
                    image.Alpha(AlphaOption.Remove);
                }

                if(image.Width > 640)
                {
                    var geo = new MagickGeometry(600, 600);
                    image.Resize(geo);
                }
                if(image.Format != MagickFormat.Jpeg)
                {
                    image.Format = MagickFormat.Jpeg;
                }
                return image.ToByteArray();
            }
        }
    }
}
