/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Covid19Radar.Api.External
{
    public class DiagnosisApi
    {
        private readonly IDiagnosisRepository Diagnosis;
        private readonly ITemporaryExposureKeyRepository Tek;
        private readonly ILogger<DiagnosisApi> Logger;

        public DiagnosisApi(
            IDiagnosisRepository diagnosis,
            ITemporaryExposureKeyRepository tek,
            ILogger<DiagnosisApi> logger)
        {
            Diagnosis = diagnosis;
            Tek = tek;
            Logger = logger;
        }

        [FunctionName("DiagnosisApi")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Diagnosis")] HttpRequest req)
        {
            Logger.LogInformation($"{nameof(DiagnosisApi)} {nameof(RunAsync)} processed a request.");

            var result = await Diagnosis.GetNotApprovedAsync();

            return new OkObjectResult(result);
        }

        [FunctionName("DiagnosisApprovedApi")]
        public async Task<IActionResult> RunApprovedAsync(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "Diagnosis/{submissionNumber}/Approved/{userUuid}")] HttpRequest req,
            string submissionNumber,
            string userUuid
            )
        {
            Logger.LogInformation($"{nameof(DiagnosisApi)} {nameof(RunApprovedAsync)} processed a request.");

            var result = await Diagnosis.GetAsync(submissionNumber, userUuid);
            foreach(var key in result.Keys)
            {
                await Tek.UpsertAsync(key);
            }

            return new OkResult();
        }

    }
}
