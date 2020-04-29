using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
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
    public class OtpSendApi
    {
        private readonly IOtpService _otpService;
        private readonly ILogger<OtpSendApi> _logger;

        public OtpSendApi(IOtpService otpService, ILogger<OtpSendApi> logger)
        {
            _otpService = otpService;
            _logger = logger;
        }

        [FunctionName(nameof(OtpSendApi))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "otp/send")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(OtpSendApi)} processed a request.");
            try
            {
                var request = await req.ParseAndThrow<OtpSendRequest>();
                await _otpService.SendAsync(request);
                return new OkResult();
            }
            catch (ArgumentException argumentException)
            {
                return new BadRequestObjectResult(argumentException.Message);
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                return new NotFoundObjectResult(unauthorizedAccessException.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"OTPSendError");
                return new InternalServerErrorResult();
            }
        }
    }
}
