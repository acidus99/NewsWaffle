using System;
using NewsWaffle.Cache;
using NewsWaffle.Net;
namespace NewsWaffle.Cgi.Media
{
	//returns optimized image bytes for a URL
	public class MediaProxy
	{
        DiskCache cache = new DiskCache(TimeSpan.FromDays(3));

        public byte [] ProxyMedia(string urlString)
        {
            Uri url = ValidateUrl(urlString);
            if(url == null)
            {
                return null;
            }
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

        private byte[] GetFromCache(Uri url)
            => cache.GetAsBytes(GetCacheKey(url));

        private void PutInCache(Uri url, byte [] data)
            => cache.Set(GetCacheKey(url), data);

        private static string GetCacheKey(Uri url)
           => url.AbsoluteUri + "optimized";

        private byte [] FetchFromNetwork(Uri url)
        {
            IHttpRequestor requestor = new HttpRequestor();

            var result = requestor.RequestAsBytes(url);
            if (result)
            {
                return requestor.BodyBytes;
            }
            else
            {
                return null;
            }
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

