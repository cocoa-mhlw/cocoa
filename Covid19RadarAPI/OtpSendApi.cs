using System;
using System.IO;
using System.Threading.Tasks;
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

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<OtpSendRequest>(requestBody);
            if (request == null || !request.IsValid())
            {
                return new BadRequestObjectResult("Invalid payload.");
            }

            await _otpService.SendAsync(request);

            return new OkObjectResult(string.Empty);
        }
    }
}
