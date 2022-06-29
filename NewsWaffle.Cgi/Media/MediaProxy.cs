using System;
using NewsWaffle.Cache;
using NewsWaffle.Net;
namespace NewsWaffle.Cgi.Media
{
	//returns optimized image bytes for a URL
	public class MediaProxy
	{
        DiskCache cache = new DiskCache(TimeSpan.FromDays(3));

        public byte [] ProxyMedia(string url)
        {
            Uri uri = ValidateUrl(url);
            if(uri == null)
            {
                return null;
            }
            url = uri.AbsoluteUri;

            //check the cache
            byte[] optimizedImage = GetFromCache(url);
            if(optimizedImage != null)
            {
                return optimizedImage;
            }

            //nope we have to go fetch it
            byte[] rawData = FetchFromNetwork(url);
            if(rawData == null)
            {
                //can't do anything
                return null;
            }

            //optimize it
            optimizedImage = MediaProcessor.ProcessImage(rawData);

            //store optimized 
            PutInCache(url, optimizedImage);
            return optimizedImage;
        }

        private byte[] GetFromCache(string url)
            => cache.GetAsBytes(GetCacheKey(url));

        private void PutInCache(string url, byte [] data)
            => cache.Set(GetCacheKey(url), data);

        private static string GetCacheKey(string url)
           => url + "optimized";

        private byte [] FetchFromNetwork(string url)
        {
            var fetcher = new HttpFetcher();
            return fetcher.GetAsBytes(url);
        }


		private static Uri ValidateUrl(string url)
        {
			try
            {
                Uri ret = new Uri(url);

                if(!ret.IsAbsoluteUri)
                {
                    return null;
                }
                if (ret.Scheme != "http" && ret.Scheme != "https")
                {
                    return null;
                }
                //TODO more checks, private ips, etc

                return ret;
            }catch(Exception)
            {

            }
            return null;
        }



	}
}

