using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Covid19Radar.Api.Services
{
    class ValidationServerService : IValidationServerService
    {
        private readonly ILogger<ValidationServerService> Logger;
        private readonly IConfiguration Config;


        public ValidationServerService(IConfiguration config, ILogger<ValidationServerService> logger)
        {
            Logger = logger;
            Config = config;
        }

        public IValidationServerService.ValidateResult Validate(HttpRequest req)
        {
            var fdidHeaderValue = req.Headers["X-Azure-FDID"].ToString();
            if (Config.AzureFrontDoorRestrictionEnabled())
            {
                if (fdidHeaderValue != Config.AzureFrontDoorId())
                {
                    return new IValidationServerService.ValidateResult()
                    {
                        IsValid = false,
                        ErrorActionResult = new StatusCodeResult(418)
                    };
                }
            }
            return new IValidationServerService.ValidateResult()
            {
                IsValid = true,
                ErrorActionResult = null
            };
        }
    }
}
