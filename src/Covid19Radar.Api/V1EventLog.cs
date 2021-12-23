/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Covid19Radar.Api
{
    public class V1EventLog
    {
        private const string HEADER_CONTENT_LENGTH = "Content-Length";

        private readonly IEventLogRepository _eventLogRepository;
        private readonly IValidationServerService _validationServerService;
        private readonly IDeviceValidationService _deviceValidationService;

        private readonly ILogger<V1EventLog> _logger;

        public V1EventLog(
            IEventLogRepository eventLogRepository,
            IValidationServerService validationServerService,
            IDeviceValidationService deviceValidationService,
            ILogger<V1EventLog> logger
            )
        {
            _eventLogRepository = eventLogRepository;
            _validationServerService = validationServerService;
            _deviceValidationService = deviceValidationService;
            _logger = logger;
        }

        [FunctionName(nameof(V1EventLog))]
        public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "v1/event_log")] HttpRequest req
        )
        {
            _logger.LogInformation($"{nameof(RunAsync)}");

            // Check Content-Length.
            long contentLength = -1;
            string contentLengthHeader = req.Headers[HEADER_CONTENT_LENGTH].ToString();
            long.TryParse(contentLengthHeader, out contentLength);

            if (contentLength < 0)
            {
                return new BadRequestErrorMessageResult("HTTP-Heaer Content-Length must be set.");
            }
            else if(contentLength > Constants.MAX_EVENT_LOG_PAYLOAD)
            {
                return new StatusCodeResult((int)HttpStatusCode.RequestEntityTooLarge);
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // Check RequestBody size.
            if (Encoding.ASCII.GetBytes(requestBody).LongLength > Constants.MAX_EVENT_LOG_PAYLOAD)
            {
                return new StatusCodeResult((int)HttpStatusCode.RequestEntityTooLarge);
            }

            // Check Valid Route
            IValidationServerService.ValidateResult validateResult = _validationServerService.Validate(req);
            if (!validateResult.IsValid)
            {
                return validateResult.ErrorActionResult;
            }

            var submissionParameter = JsonConvert.DeserializeObject<EventLogSubmissionParameter>(requestBody);
            var requestTime = DateTimeOffset.UtcNow;

            // validation device
            if (!await _deviceValidationService.Validation(submissionParameter.Platform, submissionParameter, requestTime))
            {
                _logger.LogInformation($"Invalid Device");
                return new BadRequestErrorMessageResult("Invalid Device");
            }

            var timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            using (SHA256 sha256 = SHA256.Create())
            {
                foreach (var eventLog in submissionParameter.EventLogs)
                {
                    // For considering user-privacy safe.
                    if (!eventLog.HasConsent)
                    {
                        _logger.LogError("No consent log detected.");
                        continue;
                    }

                    string id = ByteArrayUtils.ToHexString(sha256.ComputeHash(Encoding.ASCII.GetBytes(eventLog.ClearText)));

                    var eventLogModel = new EventLogModel(
                        eventLog.HasConsent,
                        eventLog.Epoch,
                        eventLog.Type,
                        eventLog.Subtype,
                        eventLog.Content,
                        eventLog.Timestamp
                        )
                    {
                        id = id,
                        Created = timestamp,
                    };
                    await _eventLogRepository.UpsertAsync(eventLogModel);
                }
            }

            return new OkObjectResult("");
        }
    }
}
