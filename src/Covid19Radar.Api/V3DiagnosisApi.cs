/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
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

namespace Covid19Radar.Api
{
    public class V3DiagnosisApi
    {
        private const string CHAFF_HEADER = "X-Chaff";

        private readonly string[] _supportRegions;

        private readonly ITemporaryExposureKeyRepository _tekRepository;
        private readonly IDeviceValidationService _deviceValidationService;
        private readonly IVerificationService _verificationService;
        private readonly IValidationServerService _validationServerService;

        private readonly ILogger<V3DiagnosisApi> _logger;

        public V3DiagnosisApi(
            IConfiguration config,
            ITemporaryExposureKeyRepository tekRepository,
            IDeviceValidationService deviceValidationService,
            IVerificationService verificationService,
            IValidationServerService validationServerService,
            ILogger<V3DiagnosisApi> logger
            )
        {
            _supportRegions = config.SupportRegions();

            _tekRepository = tekRepository;
            _deviceValidationService = deviceValidationService;
            _verificationService = verificationService;
            _validationServerService = validationServerService;
            _logger = logger;
        }

        [FunctionName(nameof(V3DiagnosisApi))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v3/diagnosis")] HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation($"{nameof(RunAsync)}");

            // Check Valid Route
            //IValidationServerService.ValidateResult validateResult = _validationServerService.Validate(req);
            //if (!validateResult.IsValid)
            //{
            //    return validateResult.ErrorActionResult;
            //}

            var submissionParameter = JsonConvert.DeserializeObject<V3DiagnosisSubmissionParameter>(requestBody);
            submissionParameter.SetDaysSinceOnsetOfSymptoms();

            // Filter valid keys
            submissionParameter.Keys = submissionParameter.Keys.Where(key => key.IsValid()).ToArray();

            var reqTime = DateTimeOffset.UtcNow;

            // payload valid
            if (!submissionParameter.IsValid())
            {
                _logger.LogInformation($"Invalid parameter");
                return new BadRequestErrorMessageResult("Invalid parameter");
            }

            // validation support region
            //if (!submissionParameter.Regions.Any(_ => _supportRegions.Contains(_)))
            //{
            //    _logger.LogInformation($"Regions not supported.");
            //    return new BadRequestErrorMessageResult("Regions not supported.");
            //}

            // validation device
            //if (!await _deviceValidationService.Validation(submissionParameter, reqTime))
            //{
            //    _logger.LogInformation($"Invalid Device");
            //    return new BadRequestErrorMessageResult("Invalid Device");
            //}

            // Check Chaff request for production
            // https://google.github.io/exposure-notifications-server/server_functional_requirements.html
            if (req.Headers?.ContainsKey(CHAFF_HEADER) ?? false)
            {
                return new OkObjectResult(JsonConvert.SerializeObject(submissionParameter));
            }

            // validatetion VerificationPayload
            //var verificationResult = await _verificationService.VerificationAsync(submissionParameter.VerificationPayload);
            //if (verificationResult != ((int)HttpStatusCode.OK))
            //{
            //    return new ObjectResult("Bad VerificationPayload") { StatusCode = verificationResult };
            //}

            var timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var keys = submissionParameter.Keys.Select(key => key.ToModel(submissionParameter, timestamp));

            foreach (var k in keys)
            {
                await _tekRepository.UpsertAsync(k);
            }

            return new OkObjectResult(JsonConvert.SerializeObject(submissionParameter));
        }
    }
}
