/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Common;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public class CustomVerificationStatusRepository : ICustomVerificationStatusRepository
    {
        private readonly ICosmos _db;
        private readonly ILogger<CustomVerificationStatusRepository> _logger;
        private readonly QueryCache<CustomVerificationStatusModel[]> GetAsyncCache;

        public CustomVerificationStatusRepository(
            ICosmos db,
            ILogger<CustomVerificationStatusRepository> logger)
        {
            _db = db;
            _logger = logger;
            GetAsyncCache = new QueryCache<CustomVerificationStatusModel[]>(Constants.CacheTimeout);
        }


        public async Task<CustomVerificationStatusModel[]> GetAsync()
        {
            _logger.LogInformation($"start {nameof(GetAsync)}");

            var items = await GetAsyncCache.QueryWithCacheAsync(async () =>
            {
                var query = _db.CustomVerificationStatus.GetItemLinqQueryable<CustomVerificationStatusModel>(true)
                    .ToFeedIterator();

                var e = Enumerable.Empty<CustomVerificationStatusModel>();
                while (query.HasMoreResults)
                {
                    var r = await query.ReadNextAsync();
                    e = e.Concat(r.Resource);
                }

                return e.ToArray();
            });

            return items.ToArray();
        }
    }
}
