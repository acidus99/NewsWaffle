using System;

namespace NewsWaffle;

public static class LinkForge
{
    public static Uri Create(string s)
    {
        try
        {
            Uri u = new Uri(s);
            if (!u.IsAbsoluteUri || u.Scheme == "file")
            {
                return null;
            }
            return u;

        }
        catch (Exception ex)
        {
            return null;
        }
    }
}

