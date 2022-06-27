using System;
using Gemini.Cgi;

namespace NewsWaffle.Cgi
{
    public static class RouteHandler
    {
        public static void Welcome(CgiWrapper cgi)
        {
            cgi.Success();
            cgi.Writer.WriteLine("# 🧇 NewsWaffle");
        }
    }
}

