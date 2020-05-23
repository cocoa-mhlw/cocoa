using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Services;
using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;

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
