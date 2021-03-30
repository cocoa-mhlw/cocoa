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
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Covid19Radar.Api
{
    public class V2DiagnosisApi
    {
        private readonly IDiagnosisRepository DiagnosisRepository;
        private readonly ITemporaryExposureKeyRepository TekRepository;
        private readonly IDeviceValidationService DeviceCheck;
        private readonly IVerificationService VerificationService;
        private readonly ILogger<V2DiagnosisApi> Logger;
        private readonly string[] SupportRegions;
        private readonly IValidationServerService ValidationServerService;

        public V2DiagnosisApi(
            IConfiguration config,
            IDiagnosisRepository diagnosisRepository,
            ITemporaryExposureKeyRepository tekRepository,
            IDeviceValidationService deviceCheck,
            IVerificationService verificationService,
            IValidationServerService validationServerService,
            ILogger<V2DiagnosisApi> logger)
        {
            DiagnosisRepository = diagnosisRepository;
            TekRepository = tekRepository;
            DeviceCheck = deviceCheck;
            Logger = logger;
            SupportRegions = config.SupportRegions();
            VerificationService = verificationService;
            ValidationServerService = validationServerService;
        }

        [FunctionName(nameof(V2DiagnosisApi))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "v2/diagnosis")] HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Logger.LogInformation($"{nameof(RunAsync)}");

            // Check Valid Route
            IValidationServerService.ValidateResult validateResult = ValidationServerService.Validate(req);
            if (!validateResult.IsValid)
            {
                return validateResult.ErrorActionResult;
            }

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

            // validation device 
            if (false == await DeviceCheck.Validation(diagnosis, reqTime))
            {
                Logger.LogInformation($"Invalid Device");
                return new BadRequestErrorMessageResult("Invalid Device");
            }

            // validatetion VerificationPayload
            var verificationResult = await VerificationService.VerificationAsync(diagnosis.VerificationPayload);
            if (verificationResult != 200)
            {
                return new ObjectResult("Bad VerificationPayload") { StatusCode = verificationResult };
            }

            var timestamp = DateTimeOffset.UtcNow;
            var keys = diagnosis.Keys.Select(_ => _.ToModel(diagnosis, (ulong)timestamp.ToUnixTimeSeconds())).ToArray();


            foreach (var k in keys)
            {
                await TekRepository.UpsertAsync(k);
            }

            return new NoContentResult();
        }
    }
}
