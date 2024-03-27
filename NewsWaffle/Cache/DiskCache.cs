using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NewsWaffle.Cache;

public class DiskCache
{
    static readonly TimeSpan DefaultLifetime = TimeSpan.FromHours(2);

    private TimeSpan Lifespan;

    public DiskCache()
    {
        Lifespan = DefaultLifetime;
    }

    public DiskCache(TimeSpan lifetime)
        : this()
    {
        Lifespan = lifetime;
    }

    public void Clear(string identifier)
    {
        //get the key
        var cacheKey = GetCacheKey(identifier);
        var filepath = GetPath(cacheKey);
        try
        {
            File.Delete(filepath);
        }
        catch (Exception)
        {
        }
    }

    public string? GetAsString(string identifier)
    {
        //get the key
        var cacheKey = GetCacheKey(identifier);
        var filepath = GetPath(cacheKey);
        try
        {
            if (IsCacheEntryValid(filepath))
            {
                return File.ReadAllText(filepath);
            }
        }
        catch (Exception)
        {
        }
        return null;
    }

    public byte[]? GetAsBytes(string identifier)
    {

        //get the key
        var cacheKey = GetCacheKey(identifier);
        var filepath = GetPath(cacheKey);
        try
        {
            if (IsCacheEntryValid(filepath))
            {
                return File.ReadAllBytes(filepath);
            }
        }
        catch (Exception)
        {
        }
        return null;
    }

    public bool Set(string identifier, string contents)
    {
        var cacheKey = GetCacheKey(identifier);
        var filepath = GetPath(cacheKey);
        try
        {
            File.WriteAllText(filepath, contents);
            return true;
        }
        catch (Exception)
        {
        }
        return false;
    }

    public bool Set(string identifier, byte[] bytes)
    {
        var cacheKey = GetCacheKey(identifier);
        var filepath = GetPath(cacheKey);
        try
        {
            File.WriteAllBytes(filepath, bytes);
            return true;
        }
        catch (Exception)
        {
        }
        return false;
    }

    private string GetPath(string cacheKey)
        => Path.Combine(Path.GetTempPath(), cacheKey + ".diskcache");

    private bool IsCacheEntryValid(string filepath)
    {
        try
        {
            return (File.GetLastWriteTime(filepath) >= DateTime.Now.Subtract(Lifespan));
        }
        catch (Exception)
        {
        }
        return false;
    }

    private string GetCacheKey(string identifier)
        => Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(identifier)));
}
