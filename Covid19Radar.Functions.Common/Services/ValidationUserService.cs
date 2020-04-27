using Covid19Radar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Covid19Radar.DataStore;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Mvc;

namespace Covid19Radar.Services
{
    public class ValidationUserService : IValidationUserService
    {
        const string AuthorizationType = "COVID-19-RADAR";
        private readonly ICosmos Cosmos;
        private readonly ICryptionService Cryption;
        private readonly ILogger<ValidationUserService> Logger;
        public ValidationUserService(
            ICosmos cosmos,
            ICryptionService cryption,
            Microsoft.Extensions.Configuration.IConfiguration config,
            ILogger<ValidationUserService> logger)
        {
            Cosmos = cosmos;
            Cryption = cryption;
            Logger = logger;
        }


        public async Task<IValidationUserService.ValidateResult> ValidateAsync(HttpRequest req, IUser user)
        {
            if (string.IsNullOrWhiteSpace(user.UserUuid)
             || string.IsNullOrWhiteSpace(user.Major)
             || string.IsNullOrWhiteSpace(user.Minor))
            {
                return IValidationUserService.ValidateResult.Error;
            }

            Microsoft.Extensions.Primitives.StringValues value;
            if (!req.Headers.TryGetValue("Authorization", out value)) return IValidationUserService.ValidateResult.Error;
            if (value.Count != 1) return IValidationUserService.ValidateResult.Error;
            var authorization = value.First();
            if (!authorization.StartsWith(AuthorizationType)) return IValidationUserService.ValidateResult.Error;
            var authorizationCode = authorization.Remove(0, AuthorizationType.Length + 1);
            return await Query(req, user, authorizationCode);
        }

        private async Task<IValidationUserService.ValidateResult> Query(HttpRequest req, IUser user, string authorizationCode)
        {
            try
            {
                var itemResult = await Cosmos.User.ReadItemAsync<UserModel>(user.GetId(), PartitionKey.None);
                if (itemResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var isValid = authorizationCode == Cryption.Unprotect(itemResult.Resource.ProtectSecret);
                    return new IValidationUserService.ValidateResult() {
                        IsValid = isValid,
                        User = itemResult.Resource,
                        ErrorActionResult = (!isValid ? new BadRequestResult() : null)
                    };
                }
            }
            catch (CosmosException ex)
            {
                // 429–TooManyRequests
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return new IValidationUserService.ValidateResult()
                    {
                        IsValid = false,
                        ErrorActionResult = new StatusCodeResult(503)
                    };
                }
                return new IValidationUserService.ValidateResult()
                {
                    IsValid = false,
                    ErrorActionResult = new StatusCodeResult((int)ex.StatusCode)
                };
            }
            return new IValidationUserService.ValidateResult()
            {
                IsValid = false,
                ErrorActionResult = new BadRequestResult()
            };
        }
    }
}
