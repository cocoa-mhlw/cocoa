using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Covid19Radar.Api
{
    public class InquiryLogApi
    {

        private readonly ILogger<InquiryLogApi> Logger;
        private readonly IInquiryLogBlobService InquiryLogBlobService;
        private readonly IValidationServerService ValidationServerService;
        private readonly IValidationInquiryLogService ValidationInquiryLogService;

        public InquiryLogApi(ILogger<InquiryLogApi> logger,
            IInquiryLogBlobService inquiryLogBlobService,
            IValidationServerService validationServerService,
            IValidationInquiryLogService validationInquiryLogService)
        {
            Logger = logger;
            InquiryLogBlobService = inquiryLogBlobService;
            ValidationServerService = validationServerService;
            ValidationInquiryLogService = validationInquiryLogService;
        }


        [FunctionName(nameof(InquiryLogApi))]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "inquirylog")] HttpRequest req)
        {
            Logger.LogInformation($"{nameof(Run)}");

            // Check Valid Route
            var serverValidateResult = ValidationServerService.Validate(req);
            if (!serverValidateResult.IsValid)
            {
                return serverValidateResult.ErrorActionResult;
            }

            // Check API key
            var inqiryLogValidateResult = ValidationInquiryLogService.Validate(req);
            if (!inqiryLogValidateResult.IsValid)
            {
                return inqiryLogValidateResult.ErrorActionResult;
            }


            var sasToken = InquiryLogBlobService.GetServiceSASToken();


            return new JsonResult(new { sas_token = sasToken });
        }
    }
}
