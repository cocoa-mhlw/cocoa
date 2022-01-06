/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Background.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Covid19Radar.Background
{
    public class TemporaryExposureKeyExportBatch
    {
        public readonly ITemporaryExposureKeyExportBatchService BatchService;

        public TemporaryExposureKeyExportBatch(ITemporaryExposureKeyExportBatchService batchService)
        {
            BatchService = batchService;
        }

        /// <summary>
        /// Four times a day, 0, 6, 12, 18 o'clock.
        /// </summary>
        /// <param name="myTimer">timer information</param>
        /// <param name="log">logger</param>
        /// <returns></returns>
        [FunctionName("TemporaryExposureKeyExportBatch")]
        public async Task Run([TimerTrigger("0 0 15 * * *")] TimerInfo myTimer, ILogger log)
        //public async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer, ILogger log)
        //public async Task Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{nameof(TemporaryExposureKeyExportBatch)} Timer trigger function executed at: {DateTime.Now}");
            await BatchService.RunAsync();
        }

        /// <summary>
        /// For test use only.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Disable("DisabledTemporaryExposureKeyExportBatchApi")]
        [FunctionName("TemporaryExposureKeyExportBatchApi")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tek_export")] HttpRequest req
            )
        {
            await BatchService.RunAsync();
            return new OkObjectResult("");
        }
    }
}
