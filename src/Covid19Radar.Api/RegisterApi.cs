/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

#nullable enable

namespace Covid19Radar.Api
{
    public class RegisterApi
    {
        private readonly ICryptionService Cryption;
        private readonly ILogger<RegisterApi> Logger;
        private readonly IUserRepository UserRepository;
        private readonly IValidationServerService ValidationServerService;

        public RegisterApi(
            IUserRepository userRepository,
            ICryptionService cryption,
            IValidationServerService validationServerService,
            ILogger<RegisterApi> logger)
        {
            Cryption = cryption;
            Logger = logger;
            UserRepository = userRepository;
            ValidationServerService = validationServerService;
        }

        [FunctionName(nameof(RegisterApi))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "register")] HttpRequest req)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            // Check Valid Route
            IValidationServerService.ValidateResult validateResult = ValidationServerService.Validate(req);
            if (!validateResult.IsValid)
            {
                return validateResult.ErrorActionResult;
            }

            // UserUuid
            var userUuid = Guid.NewGuid().ToString("N")
                + DateTime.UtcNow.Ticks.ToString();

            // save to DB
            return await Register(userUuid);
        }

        private async Task<IActionResult> Register(string userUuid)
        {
            var newItem = new UserModel();
            var secret = Cryption.CreateSecret(userUuid);
            newItem.UserUuid = userUuid;
            newItem.ProtectSecret = Cryption.Protect(secret);
            await UserRepository.Create(newItem);
            var result = new RegisterResultModel();
            result.UserUuid = userUuid;
            result.Secret = secret;
            result.JumpConsistentSeed = newItem.JumpConsistentSeed;
            return new OkObjectResult(result);
        }
    }
}
