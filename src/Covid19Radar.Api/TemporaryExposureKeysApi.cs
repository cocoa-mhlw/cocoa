/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19Radar.Api
{
    public class TemporaryExposureKeysApi
    {
        private readonly ITemporaryExposureKeyExportRepository TekExport;
        private readonly ILogger<TemporaryExposureKeysApi> Logger;
        private readonly string ExportKeyUrl;
        private readonly string TekExportBlobStorageContainerPrefix;
        private readonly IValidationServerService ValidationServerService;

        public TemporaryExposureKeysApi(
            IConfiguration config,
            ITemporaryExposureKeyExportRepository tekExportRepository,
            IValidationServerService validationServerService,
            ILogger<TemporaryExposureKeysApi> logger
            )
        {
            Logger = logger;
            TekExport = tekExportRepository;
            ExportKeyUrl = config.ExportKeyUrl();
            TekExportBlobStorageContainerPrefix = config.TekExportBlobStorageContainerPrefix();
            ValidationServerService = validationServerService;
        }

        [FunctionName(nameof(TemporaryExposureKeysApi))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "TemporaryExposureKeys")] HttpRequest req)
        {
            // Check Valid Route
            IValidationServerService.ValidateResult validateResult = ValidationServerService.Validate(req);
            if (!validateResult.IsValid)
            {
                return validateResult.ErrorActionResult;
            }

            if (!long.TryParse(req.Query?["since"], out var sinceEpochSeconds))
                sinceEpochSeconds = new DateTimeOffset(DateTime.UtcNow.AddDays(-14)).ToUnixTimeSeconds();

            var keysResponse = await TekExport.GetKeysAsync((ulong)sinceEpochSeconds);
            var result = new TemporaryExposureKeysResult();
            // TODO: Url util
            result.Keys = keysResponse
                .Select(_ => new TemporaryExposureKeysResult.Key() { Url = $"{ExportKeyUrl}/{TekExportBlobStorageContainerPrefix}/{_.Region}/{_.BatchNum}.zip" });
            result.Timestamp = keysResponse
                .OrderByDescending(_ => _.TimestampSecondsSinceEpoch)
                .FirstOrDefault()?.TimestampSecondsSinceEpoch ?? sinceEpochSeconds;
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(TemporaryExposureKeysApi) + "WithRegion")]
        public async Task<IActionResult> RunWithRegionAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "TemporaryExposureKeys/{region}")] HttpRequest req,
            string region)
        {
            // Check Valid Route
            IValidationServerService.ValidateResult validateResult = ValidationServerService.Validate(req);
            if (!validateResult.IsValid)
            {
                return validateResult.ErrorActionResult;
            }

            if (!long.TryParse(req.Query?["since"], out var sinceEpochSeconds))
                sinceEpochSeconds = new DateTimeOffset(DateTime.UtcNow.AddDays(-14)).ToUnixTimeSeconds();

            var keysResponse = await TekExport.GetKeysAsync((ulong)sinceEpochSeconds, region);
            var result = new TemporaryExposureKeysResult();
            // TODO: Url util
            result.Keys = keysResponse
                .Select(_ => new TemporaryExposureKeysResult.Key() { Url = $"{ExportKeyUrl}/{TekExportBlobStorageContainerPrefix}/{_.Region}/{_.BatchNum}.zip" });
            result.Timestamp = keysResponse
                .OrderByDescending(_ => _.TimestampSecondsSinceEpoch)
                .FirstOrDefault()?.TimestampSecondsSinceEpoch ?? sinceEpochSeconds;
            return new OkObjectResult(result);
        }
    }
}
