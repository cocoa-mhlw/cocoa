using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Covid19Radar.DataAccess;
using Covid19Radar.Services;
using Covid19Radar.Models;
using System.Linq;

namespace Covid19Radar
{
    public class DiagnosisApi
    {
        private readonly IDiagnosisRepository DiagnosisRepository;
        private readonly IValidationUserService Validation;
        private readonly ILogger<DiagnosisApi> Logger;

        public DiagnosisApi(
            IDiagnosisRepository diagnosisRepository,
            IValidationUserService validation,
            ILogger<DiagnosisApi> logger)
        {
            DiagnosisRepository = diagnosisRepository;
            Validation = validation;
            Logger = logger;
        }

        [FunctionName(nameof(DiagnosisApi))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "diagnosis")] HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var diagnosis = JsonConvert.DeserializeObject<DiagnosisSubmissionParameter>(requestBody);

            // validation
            var validationResult = await Validation.ValidateAsync(req, diagnosis);
            if (!validationResult.IsValid)
            {
                return validationResult.ErrorActionResult;
            }

            var timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // TODO: Device verify

            await DiagnosisRepository.SubmitDiagnosisAsync(
                diagnosis.SubmissionNumber,
                diagnosis.UserUuid,
                diagnosis.Keys.Select(_ => _.ToModel(diagnosis, timestamp)).ToArray());

            return new OkResult();
        }
    }
}
