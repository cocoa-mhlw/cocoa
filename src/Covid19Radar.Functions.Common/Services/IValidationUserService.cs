using Covid19Radar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface IValidationUserService
    {

        Task<ValidateResult> ValidateAsync(HttpRequest req, IUser user);

        public struct ValidateResult
        {
            public static readonly ValidateResult Error = new ValidateResult()
            {
                IsValid = false,
                ErrorActionResult = new BadRequestResult()
            };
            public bool IsValid;
            public UserModel User;
            public IActionResult ErrorActionResult;
        }

    }
}
