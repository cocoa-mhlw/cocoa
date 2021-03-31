/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Covid19Radar.Api.Services
{
    public class ValidationInquiryLogService : IValidationInquiryLogService
    {
        private readonly ILogger<ValidationInquiryLogService> Logger;
        private readonly IConfiguration Config;

        public ValidationInquiryLogService(ILogger<ValidationInquiryLogService> logger,
            IConfiguration config)
        {
            Logger = logger;
            Config = config;

            Logger.LogInformation($"{nameof(ValidationInquiryLogService)} constructor");
        }

        public IValidationInquiryLogService.ValidateResult Validate(HttpRequest req)
        {
            Logger.LogInformation($"{nameof(ValidationInquiryLogService)} {nameof(Validate)}");
            var test = Config.InquiryLogApiKey();
            var test2 = req.Headers["x-api-key"].ToString();
            var test3 = req.Headers.ContainsKey("x-api-key");
            if (!req.Headers.ContainsKey("x-api-key") || Config.InquiryLogApiKey() != req.Headers["x-api-key"].ToString())
            {
                return IValidationInquiryLogService.ValidateResult.Error;
            }

            return IValidationInquiryLogService.ValidateResult.Success;
        }
    }
}
