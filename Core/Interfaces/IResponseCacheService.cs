using System;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IResponseCacheService
    {
         Task CacheResponseAsync(string cachekey, object response, TimeSpan timeToLive);
         Task<String> GetCacheResponseAsync(string cacheKey);
    }
}