using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Covid19Radar.Services;
using Covid19Radar.DataAccess;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Covid19Radar.Models;
using Microsoft.Extensions.Configuration;

namespace Covid19Radar
{
    public class TemporaryExposureKeysApi
    {
        private readonly ITemporaryExposureKeyExportRepository TekExport;
        private readonly ICryptionService Cryption;
        private readonly ILogger<TemporaryExposureKeysApi> Logger;
        private readonly string ExportKeyUrl;
        private readonly string TekExportBlobStorageContainerPrefix;

        public TemporaryExposureKeysApi(
            ITemporaryExposureKeyExportRepository tekExportRepository,
            ICryptionService cryption,
            ILogger<TemporaryExposureKeysApi> logger,
            IConfiguration config)
        {
            Cryption = cryption;
            Logger = logger;
            TekExport = tekExportRepository;
            ExportKeyUrl = config["ExportKeyUrl"];
            TekExportBlobStorageContainerPrefix = config["TekExportBlobStorageContainerPrefix"];
        }

        [FunctionName(nameof(TemporaryExposureKeysApi))]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "TemporaryExposureKeys")] HttpRequest req)
		{
			if (!long.TryParse(req.Query?["since"], out var sinceEpochSeconds))
				sinceEpochSeconds = new DateTimeOffset(DateTime.UtcNow.AddDays(-14)).ToUnixTimeSeconds();

            var keysResponse = await TekExport.GetKeysAsync((ulong)sinceEpochSeconds);
            var result = new TemporaryExposureKeysResult();
            // TODO: Url util
            result.Keys = keysResponse.Select(_ => new TemporaryExposureKeysResult.Key() { Url = $"{ExportKeyUrl}/{TekExportBlobStorageContainerPrefix}/{_.BatchNum}.zip" });
            result.Timestamp = keysResponse
                .OrderByDescending(_ => _.TimestampSecondsSinceEpoch)
                .FirstOrDefault()?.TimestampSecondsSinceEpoch ?? sinceEpochSeconds;
            return new OkObjectResult(result);
		}
	}
}
