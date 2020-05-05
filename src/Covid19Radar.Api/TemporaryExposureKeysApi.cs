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

namespace Covid19Radar
{
    public class TemporaryExposureKeysApi
    {
        private readonly ITemporaryExposureKeyRepository TekRepository;
        private readonly ICryptionService Cryption;
        private readonly ILogger<TemporaryExposureKeysApi> Logger;

        public TemporaryExposureKeysApi(
            ITemporaryExposureKeyRepository tekRepository,
            ICryptionService cryption,
            ILogger<TemporaryExposureKeysApi> logger)
        {
            Cryption = cryption;
            Logger = logger;
            TekRepository = tekRepository;
        }

        [FunctionName(nameof(TemporaryExposureKeysApi))]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "TemporaryExposureKeys")] HttpRequest req)
		{
			if (!long.TryParse(req.Query?["since"], out var sinceEpochSeconds))
				sinceEpochSeconds = new DateTimeOffset(DateTime.UtcNow.AddDays(-14)).ToUnixTimeSeconds();

			var keysResponse = await TekRepository.GetKeysAsync(sinceEpochSeconds);

			return new OkObjectResult(keysResponse);
		}
	}
}
