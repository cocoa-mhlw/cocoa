using Covid19Radar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public class ValidationUserService : IValidationUserService
    {
        private readonly ILogger<ValidationUserService> Logger;
        public ValidationUserService(
            Microsoft.Extensions.Configuration.IConfiguration config,
            ILogger<ValidationUserService> logger)
        {
            this.Logger = logger;
        }

        public async Task<bool> ValidateAsync(HttpRequest req, IUser user)
        {
            Microsoft.Extensions.Primitives.StringValues value;
            if (!req.Headers.TryGetValue("Authorization", out value)) return false;
            if (value.Count != 1) return false;
            return true;
        }

    }
}
