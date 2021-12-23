using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
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
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IValidationServerService _validationServerService;
        private readonly IDeviceValidationService _deviceValidationService;

        private readonly ILogger<RegisterApi> _logger;

        public V1EventLog(
            IEventLogRepository eventLogRepository,
            IValidationServerService validationServerService,
            IDeviceValidationService deviceValidationService,
            ILogger<RegisterApi> logger
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
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation($"{nameof(RunAsync)}");

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

            foreach (var eventLog in submissionParameter.EventLogs)
            {
                // For considering user-privacy safe.
                if (!eventLog.HasConsent)
                {
                    _logger.LogError("No consent log detected.");
                    continue;
                }

                eventLog.Created = requestTime.ToUnixTimeSeconds();
                await _eventLogRepository.UpsertAsync(eventLog);
            }

            return new OkObjectResult("");
        }

    }
}
