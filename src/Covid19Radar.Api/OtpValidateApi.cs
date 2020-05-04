using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Covid19Radar.Common;
using Covid19Radar.Extensions;
using Covid19Radar.Models;
using Covid19Radar.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Covid19Radar
{
    public class OtpValidateApi
    {
        private readonly IOtpService _otpService;
        private readonly IValidationUserService _validation;
        private readonly ILogger<OtpValidateApi> _logger;

        public OtpValidateApi(
            IOtpService otpService,
            IValidationUserService validation,
            ILogger<OtpValidateApi> logger)
        {
            _otpService = otpService;
            _validation = validation;
            _logger = logger;
        }

        [FunctionName(nameof(OtpValidateApi))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "otp/validate")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(OtpSendApi)} processed a request.");
            
            try
            {
                var request = await req.ParseAndThrow<OtpValidateRequest>();

                // validation
                var validationResult = await _validation.ValidateAsync(req, request.User);
                if (!validationResult.IsValid)
                {
                    return validationResult.ErrorActionResult;
                }

                var validOtp = await _otpService.ValidateAsync(request);
                return new OkObjectResult(new {valid = validOtp});
            }
            catch (ArgumentException argumentException)
            {
                return new BadRequestObjectResult(argumentException.Message);
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                return new NotFoundObjectResult(unauthorizedAccessException.Message);
            }
            catch (Exception exception)
            {
                Console.Write(exception);
                return new InternalServerErrorResult();
            }
        }
    }
}
