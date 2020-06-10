using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Covid19Radar.Api
{
    public class DiagnosisApi
    {
        private readonly IDiagnosisRepository DiagnosisRepository;
        private readonly ITemporaryExposureKeyRepository TekRepository;
        private readonly IValidationUserService Validation;
        private readonly IDeviceValidationService DeviceCheck;
        private readonly IVerificationService VerificationService;
        private readonly ILogger<DiagnosisApi> Logger;
        private readonly string[] SupportRegions;

        public DiagnosisApi(
            IConfiguration config,
            IDiagnosisRepository diagnosisRepository,
            ITemporaryExposureKeyRepository tekRepository,
            IValidationUserService validation,
            IDeviceValidationService deviceCheck,
            IVerificationService verificationService,
            ILogger<DiagnosisApi> logger)
        {
            DiagnosisRepository = diagnosisRepository;
            TekRepository = tekRepository;
            Validation = validation;
            DeviceCheck = deviceCheck;
            Logger = logger;
            SupportRegions = config.SupportRegions();
            VerificationService = verificationService;
        }

        [FunctionName(nameof(DiagnosisApi))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "diagnosis")] HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Logger.LogInformation($"{nameof(RunAsync)} request body {requestBody}");
            var diagnosis = JsonConvert.DeserializeObject<DiagnosisSubmissionParameter>(requestBody);
            var reqTime = DateTimeOffset.UtcNow;

            // payload valid
            if (!diagnosis.IsValid())
            {
                Logger.LogInformation($"Invalid parameter");
                return new BadRequestErrorMessageResult("Invalid parameter");
            }

            // validation support region
            if (!diagnosis.Regions.Any(_ => SupportRegions.Contains(_)))
            {
                Logger.LogInformation($"Regions not supported.");
                return new BadRequestErrorMessageResult("Regions not supported.");
            }

            // validation
            var validationResult = await Validation.ValidateAsync(req, diagnosis);
            if (!validationResult.IsValid)
            {
                Logger.LogInformation($"validation error.");
                return validationResult.ErrorActionResult;
            }

            // validation device 
            if (false == await DeviceCheck.Validation(diagnosis, reqTime))
            {
                Logger.LogInformation($"Invalid Device");
                return new BadRequestErrorMessageResult("Invalid Device");
            }

            // validatetion VerificationPayload
            if (!await VerificationService.Verification(diagnosis.VerificationPayload))
            {
                return new ObjectResult("Bad VerificationPayload") { StatusCode = 406 };
            }
            //// TODO: validatetion VerificationPayload connnection error 5xx
            //if (false == true)
            //{
            //    return new ObjectResult("Unable to communicate with center") { StatusCode = 503 };
            //}

            var timestamp = DateTimeOffset.UtcNow;
            var keys = diagnosis.Keys.Select(_ => _.ToModel(diagnosis, (ulong)timestamp.ToUnixTimeSeconds())).ToArray();

            await DiagnosisRepository.SubmitDiagnosisAsync(
                diagnosis.VerificationPayload,
                timestamp,
                diagnosis.UserUuid,
                keys);

            foreach (var k in keys)
            {
                await TekRepository.UpsertAsync(k);
            }

            return new NoContentResult();
        }
    }
}
