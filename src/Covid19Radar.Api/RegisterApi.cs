using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Covid19Radar.Models;
using Covid19Radar.DataStore;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Covid19Radar.Services;
using Covid19Radar.DataAccess;

#nullable enable

namespace Covid19Radar.Api
{
    public class RegisterApi
    {
        private readonly ICryptionService Cryption;
        private readonly ILogger<RegisterApi> Logger;
        private readonly IUserRepository UserRepository;

        public RegisterApi(
            IUserRepository userRepository,
            ICryptionService cryption,
            ILogger<RegisterApi> logger)
        {
            Cryption = cryption;
            Logger = logger;
            UserRepository = userRepository;
        }

        [FunctionName(nameof(RegisterApi))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "register")] HttpRequest req)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            // UserUuid
            var userUuid = Guid.NewGuid().ToString("N")
                + DateTime.UtcNow.Ticks.ToString();

            // save to DB
            return await Register(userUuid);
        }

        private async Task<IActionResult> Register(string userUuid)
        {
            var number = await UserRepository.NextSequenceNumber();
            // 503 Error, Fail get number
            if (number == null)
            {
                return new StatusCodeResult(503);
            }

            var newItem = new UserModel();
            var secret = Cryption.CreateSecret();
            newItem.UserUuid = userUuid;
            newItem.Major = number.Major.ToString();
            newItem.Minor = number.Minor.ToString();
            newItem.SetStatus(Common.UserStatus.None);
            newItem.ProtectSecret = Cryption.Protect(secret);
            await UserRepository.Create(newItem);
            var result = new RegisterResultModel();
            result.UserUuid = userUuid;
            result.Major = newItem.Major;
            result.Minor = newItem.Minor;
            result.Secret = secret;
            return new OkObjectResult(result);
        }
    }
}
