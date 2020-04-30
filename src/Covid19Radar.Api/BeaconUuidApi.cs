using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using System.Linq;
using Covid19Radar.DataStore;
using Covid19Radar.Models;
using System.Runtime.Caching;

namespace Covid19Radar.Api
{
    public class BeaconUuidApi
    {
        private readonly ICosmos Cosmos;
        private readonly ILogger<BeaconUuidApi> Logger;
        static readonly ObjectCache memoryCache = MemoryCache.Default;

        public BeaconUuidApi(ICosmos cosmos, ILogger<BeaconUuidApi> logger)
        {
            Cosmos = cosmos;
            Logger = logger;
        }

        [FunctionName("BeaconUuid")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            switch (req.Method)
            {
                case "POST":
                    return await Post(req);
                case "GET":
                    return await Post(req);
            }

            AddBadRequest(req);
            return new BadRequestObjectResult("");

        }

        private async Task<IActionResult> Post(HttpRequest req)
        {
            return await Query(req);
        }

        private async Task<IActionResult> Query(HttpRequest req)
        {
            var now = DateTime.UtcNow;
            var key = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, TimeSpan.Zero).ToString("yyyyMMddHHmm");

            CacheItem cachedContent = memoryCache.GetCacheItem(key);

            if (cachedContent == null)
            {
                try
                {
                    var itemResult = await Cosmos.BeaconUuid.ReadItemAsync<BeaconUuidModel>(key, PartitionKey.None);
                    if (itemResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        CacheItemPolicy cachePolicy = new CacheItemPolicy();
                        cachePolicy.Priority = CacheItemPriority.Default;
                        cachePolicy.AbsoluteExpiration = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, TimeSpan.Zero).AddHours(2);
                        cachedContent = new CacheItem(key, itemResult.Resource);
                        memoryCache.Set(cachedContent, cachePolicy);

                        return new OkObjectResult(itemResult.Resource);
                    }
                }
                catch (CosmosException ex)
                {
                    // 429–TooManyRequests
                    if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        return new StatusCodeResult(503);
                    }
                    else if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Create New
                        var beaconUuid = new BeaconUuidModel(now);
                        var result = await Cosmos.BeaconUuid.CreateItemAsync<BeaconUuidModel>(beaconUuid);
                        return new OkObjectResult(beaconUuid);
                    }
                    AddBadRequest(req);
                    return new StatusCodeResult((int)ex.StatusCode);
                }
                AddBadRequest(req);
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(cachedContent.Value);
            }
        }
        private void AddBadRequest(HttpRequest req)
        {
            // add deny list
        }
    }
}
