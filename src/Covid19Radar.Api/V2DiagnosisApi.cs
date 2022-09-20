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
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Covid19Radar.Api
{
    public class V2DiagnosisApi
    {
        private const string CHAFF_HEADER = "X-Chaff";
        private readonly ITemporaryExposureKeyRepository TekRepository;
        private readonly IDeviceValidationService DeviceCheck;
        private readonly IVerificationService VerificationService;
        private readonly ILogger<V2DiagnosisApi> Logger;
        private readonly string[] SupportRegions;
        private readonly IValidationServerService ValidationServerService;

        public V2DiagnosisApi(
            IConfiguration config,
            ITemporaryExposureKeyRepository tekRepository,
            IDeviceValidationService deviceCheck,
            IVerificationService verificationService,
            IValidationServerService validationServerService,
            ILogger<V2DiagnosisApi> logger)
        {
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

            return new StatusCodeResult((int)HttpStatusCode.NotAcceptable);
        }
    }
}
