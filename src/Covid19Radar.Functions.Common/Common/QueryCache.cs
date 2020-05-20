using System;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Common
{
    public class QueryCache<T>
    {
        private readonly int CacheTimeout = 60;
        private readonly EventWaitHandle Event = new EventWaitHandle(true, EventResetMode.AutoReset);
        private T Cache;
        private long Timestamp = 0;

        public QueryCache(int cacheTimeout)
        {
            CacheTimeout = cacheTimeout;
        }


        public async Task<T> QueryWithCacheAsync(Func<Task<T>> query)
        {
            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if ((Timestamp + CacheTimeout) < time)
            {
                try
                {
                    Event.WaitOne();
                    if ((Timestamp + CacheTimeout) < time)
                    {
                        Cache = await query();
                        Timestamp = time;
                    }
                }
                finally
                {
                    Event.Set();
                }
            }
            return Cache;
        }
    }
}
