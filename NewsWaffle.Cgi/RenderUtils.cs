using System;
namespace NewsWaffle.Cgi
{
	public static class RenderUtils
	{
        public static string Savings(int newSize, int originalSize)
            => string.Format("{0:0.00}%", (1.0d - (Convert.ToDouble(newSize) / Convert.ToDouble(originalSize))) * 100.0d);

        public static string ReadableFileSize(double size, int unit = 0)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            while (size >= 1024)
            {
                size /= 1024;
                ++unit;
            }

            return string.Format("{0:0.0#} {1}", size, units[unit]);
        }
    }
}

