using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covid19Radar.Api.Services
{
    public interface IValidationInquiryLogService
    {
        ValidateResult Validate(HttpRequest req);
        public class ValidateResult
        {
            public static readonly ValidateResult Success = new ValidateResult()
            {
                IsValid = true,
                ErrorActionResult = null
            };
            public static readonly ValidateResult Error = new ValidateResult()
            {
                IsValid = false,
                ErrorActionResult = new BadRequestResult()
            };
            public bool IsValid;
            public IActionResult ErrorActionResult;
        }
    }
}
