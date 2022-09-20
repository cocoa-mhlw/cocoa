/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Covid19Radar.Api.Common;
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
        private const int TRANSMISSION_RISK_LEVEL_LOWEST = 1;
        private const int TRANSMISSION_RISK_LEVEL_MEDIUM = 4;

        private const string CHAFF_HEADER = "X-Chaff";

        private readonly string[] _supportRegions;

        private readonly ITemporaryExposureKeyRepository _tekRepository;
        private readonly IDeviceValidationService _deviceValidationService;
        private readonly IVerificationService _verificationService;
        private readonly IValidationServerService _validationServerService;
        private readonly ITemporaryExposureKeyValidationService _temporaryExposureKeyValidationService;

        private readonly ILogger<V3DiagnosisApi> _logger;

        public V3DiagnosisApi(
            IConfiguration config,
            ITemporaryExposureKeyRepository tekRepository,
            IDeviceValidationService deviceValidationService,
            IVerificationService verificationService,
            IValidationServerService validationServerService,
            ITemporaryExposureKeyValidationService temporaryExposureKeyValidationService,
            ILogger<V3DiagnosisApi> logger
            )
        {
            _supportRegions = config.SupportRegions();

            _tekRepository = tekRepository;
            _deviceValidationService = deviceValidationService;
            _verificationService = verificationService;
            _validationServerService = validationServerService;
            _temporaryExposureKeyValidationService = temporaryExposureKeyValidationService;
            _logger = logger;
        }

        [FunctionName(nameof(V3DiagnosisApi))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "v3/diagnosis")] HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation($"{nameof(RunAsync)}");

            // Check Valid Route
            IValidationServerService.ValidateResult validateResult = _validationServerService.Validate(req);
            if (!validateResult.IsValid)
            {
                return validateResult.ErrorActionResult;
            }

            return new StatusCodeResult((int)HttpStatusCode.NotAcceptable);
        }
    }
}
