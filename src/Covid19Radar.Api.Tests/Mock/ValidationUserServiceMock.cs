using Covid19Radar.Models;
using Covid19Radar.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.Mock
{
    public class ValidationUserServiceMock : IValidationUserService
    {
        public Task<IValidationUserService.ValidateResult> ValidateAsync(HttpRequest req, IUser user)
        {
            return Task.FromResult(new IValidationUserService.ValidateResult() { IsValid = true, User = new UserModel(), ErrorActionResult = null });
        }
    }
}
