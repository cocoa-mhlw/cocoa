using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19Radar.Api
{
    public class DiagnosisApi
    {
        private readonly IDiagnosisRepository DiagnosisRepository;
        private readonly IValidationUserService Validation;
        private readonly IDeviceValidationService DeviceCheck;
        private readonly ILogger<DiagnosisApi> Logger;

        public DiagnosisApi(
            IDiagnosisRepository diagnosisRepository,
            IValidationUserService validation,
            IDeviceValidationService deviceCheck,
            ILogger<DiagnosisApi> logger)
        {
            DiagnosisRepository = diagnosisRepository;
            Validation = validation;
            DeviceCheck = deviceCheck;
            Logger = logger;
        }

        [FunctionName(nameof(DiagnosisApi))]
        public async Task<IActionResult> RunAsync(
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

            // Device validation
            if (false == await DeviceCheck.Validation(diagnosis))
            {
                return validationResult.ErrorActionResult;
            }

            await DiagnosisRepository.SubmitDiagnosisAsync(
                diagnosis.SubmissionNumber,
                diagnosis.UserUuid,
                diagnosis.Keys.Select(_ => _.ToModel(diagnosis, timestamp)).ToArray());

            return new OkResult();
        }
    }
}
