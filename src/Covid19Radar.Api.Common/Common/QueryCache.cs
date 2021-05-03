﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Common
{
    /// <summary>
    /// Closure caching optimized for queries
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    public class QueryCache<T>
    {
        private readonly int CacheTimeout = Constants.CacheTimeout;
        private readonly EventWaitHandle Event = new EventWaitHandle(true, EventResetMode.AutoReset);
        private T Cache;
        private long Timestamp = 0;

        /// <summary>
        /// Constructors that receive cache timeouts
        /// </summary>
        /// <param name="cacheTimeout">cache timeout</param>
        public QueryCache(int cacheTimeout)
        {
            CacheTimeout = cacheTimeout;
        }

        /// <summary>
        /// Returns cache if cache is enabled.
        /// if cache is invalid, executes the query to cache the results.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<T> QueryWithCacheAsync(Func<Task<T>> query)
        {
            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if ((Timestamp + CacheTimeout) <= time)
            {
                try
                {
                    Event.WaitOne();
                    if ((Timestamp + CacheTimeout) <= time)
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
