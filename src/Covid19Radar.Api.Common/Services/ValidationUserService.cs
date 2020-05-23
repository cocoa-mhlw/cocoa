using Covid19Radar.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Covid19Radar.Api.DataStore;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Mvc;

namespace Covid19Radar.Api.Services
{
    public class ValidationUserService : IValidationUserService
    {
        const string AuthorizationType = "Bearer";
        private readonly ICosmos Cosmos;
        private readonly ICryptionService Cryption;
        private readonly ILogger<ValidationUserService> Logger;
        public ValidationUserService(
            ICosmos cosmos,
            ICryptionService cryption,
            ILogger<ValidationUserService> logger)
        {
            Cosmos = cosmos;
            Cryption = cryption;
            Logger = logger;
        }


        public async Task<IValidationUserService.ValidateResult> ValidateAsync(HttpRequest req, IUser user)
        {
            if (string.IsNullOrWhiteSpace(user.UserUuid))
            {
                return IValidationUserService.ValidateResult.Error;
            }

            Microsoft.Extensions.Primitives.StringValues value;
            if (!req.Headers.TryGetValue("Authorization", out value)) return IValidationUserService.ValidateResult.Error;
            if (value.Count != 1) return IValidationUserService.ValidateResult.Error;
            var authorization = value.FirstOrDefault();
            if (string.IsNullOrEmpty(authorization)) return IValidationUserService.ValidateResult.Error;
            var splited = authorization.Split(' ');
            if (splited.Length != 2) return IValidationUserService.ValidateResult.Error;
            var authorizationType = splited[0];
            var authorizationCode = splited[1];
            if (authorizationType != AuthorizationType) return IValidationUserService.ValidateResult.Error;

            if (Cryption.ValidateSecret(user.UserUuid, authorizationCode))
            {
                return new IValidationUserService.ValidateResult()
                {
                    IsValid = true,
                    User = null,
                    ErrorActionResult = null
                };
            }
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
                    return new IValidationUserService.ValidateResult()
                    {
                        IsValid = isValid,
                        User = (isValid ? itemResult.Resource : null),
                        ErrorActionResult = (isValid ? null : new BadRequestResult())
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
