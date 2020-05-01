using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public class InfectionService : IInfectionService
    {
        private readonly ICosmos Cosmos;
        private readonly ILogger<InfectionService> Logger;

        private int LastCheckInterval;
        private DateTime _LastUpdateTime = DateTime.MinValue;
        private DateTime _LastUpdateCheckTime = DateTime.MinValue;
        private object LastCheckLock = new object();
        private object LastLock = new object();
        private DateTime LastGetTime = DateTime.MinValue;
        private InfectionModel[] Infections = new InfectionModel[] { };

        public DateTime LastUpdateTime
        {
            get
            {
                RefreshTime();
                return _LastUpdateTime;
            }
        }

        public InfectionService(
            Microsoft.Extensions.Configuration.IConfiguration config,
            ICosmos cosmos,
            ILogger<InfectionService> logger)
        {
            this.Cosmos = cosmos;
            this.Logger = logger;
            this.LastCheckInterval = Convert.ToInt32(config.GetSection("LAST_NOTIFICATION_CHECK_INTERVAL").Value);
        }

        private void RefreshTime()
        {
            if ((DateTime.UtcNow - _LastUpdateCheckTime).TotalSeconds > LastCheckInterval)
            {
                lock (LastCheckLock)
                {
                    if ((DateTime.UtcNow - _LastUpdateCheckTime).TotalSeconds > LastCheckInterval)
                    {
                        var qTask = QueryLastUpdateTime();
                        qTask.Wait();
                        _LastUpdateTime = qTask.Result;
                        _LastUpdateCheckTime = DateTime.UtcNow;
                    }
                }
            }
        }
        private async Task<DateTime> QueryLastUpdateTime()
        {
            var q = new QueryDefinition("SELECT * FROM c ORDER BY c.Updated DESC");
            var opt = new QueryRequestOptions();
            opt.MaxItemCount = 1;
            var iterator = Cosmos.Infection.GetItemQueryIterator<InfectionModel>(q, null, opt);
            var result = await iterator.ReadNextAsync();
            return result.Resource.FirstOrDefault()?.Updated ?? DateTime.MinValue;
        }

        public IEnumerable<InfectionModel> GetList(DateTime lastClientTime, out DateTime lastUpdateTime)
        {
            lastUpdateTime = LastUpdateTime;
            if ((DateTime.UtcNow - LastGetTime).TotalSeconds > LastCheckInterval)
            {
                lock (LastLock)
                {
                    if ((DateTime.UtcNow - LastGetTime).TotalSeconds > LastCheckInterval)
                    {
                        var qTask = Query(LastGetTime);
                        Infections = qTask.Result
                            .OrderByDescending(_ => _.Updated)
                            .ToArray();
                        LastGetTime = DateTime.UtcNow;
                    }
                }
            }
            return Infections.Where(_ => _.Updated > lastClientTime);
        }

        private async Task<IEnumerable<InfectionModel>> Query(DateTime lastScanTime)
        {
            var q = new QueryDefinition("SELECT * FROM c ");
            //q.WithParameter("@Updated", lastScanTime.AddMinutes(-1));
            var opt = new QueryRequestOptions();
            var iterator = Cosmos.Infection.GetItemQueryIterator<InfectionModel>(q, null, opt);
            var result = Enumerable.Empty<InfectionModel>();
            while (iterator.HasMoreResults)
            {
                result = result.Concat(await iterator.ReadNextAsync());
            }
            return result;
        }

    }
}
