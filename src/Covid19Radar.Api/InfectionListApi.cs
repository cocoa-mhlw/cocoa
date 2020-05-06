using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Covid19Radar.Models;
using Covid19Radar.DataStore;
using Covid19Radar.Services;
using Covid19Radar.DataAccess;
using System.Linq;

#nullable enable

namespace Covid19Radar.Api
{
    [Obsolete]
    public class InfectionListApi
    {
        private readonly IUserRepository UserRepository;
        private readonly IInfectionService Infection;
        private readonly IValidationUserService Validation;
        private readonly ILogger<InfectionListApi> Logger;

        public InfectionListApi(
            IUserRepository userRepository,
            IInfectionService infection,
            IValidationUserService validation,
            ILogger<InfectionListApi> logger)
        {
            UserRepository = userRepository;
            Infection = infection;
            Validation = validation;
            Logger = logger;
        }

        [FunctionName(nameof(InfectionListApi))]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "infection/list/{lastClientUpdateTime:datetime}")] HttpRequest req,
            DateTime lastClientUpdateTime)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            // Infection
            var result = new InfectionListResult();
            DateTime lastUpdate;
            result.List = Infection.GetList(lastClientUpdateTime, out lastUpdate)
                .Select(_ => new InfectionListResult.Item()
                {
                    Major = _.Major,
                    Minor = _.Minor,
                    ImpactStart = _.ImpactStart,
                    ImpactEnd = _.ImpactEnd
                })
                .ToArray();
            result.LastUpdateTime = lastUpdate;

            // query
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(InfectionListApi) + "Deprecated")]
        public async Task<IActionResult> RunDeprecated(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Infection/List/{UserUuid}/{Major}/{Minor}/{LastTime:datetime}")] HttpRequest req,
            string userUuid,
            string major,
            string minor,
            DateTime lastTime)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            var user = new UserParameter() { UserUuid = userUuid, Major = major, Minor = minor };

            // validation
            var validationResult = await Validation.ValidateAsync(req, user);
            if (!validationResult.IsValid)
            {
                AddBadRequest(req);
                return validationResult.ErrorActionResult;
            }

            // Infection
            var result = new InfectionListResult();
            DateTime lastUpdate;
            result.List = Infection.GetList(lastTime, out lastUpdate)
                .Select(_ => new InfectionListResult.Item()
                {
                    Major = _.Major,
                    Minor = _.Minor,
                    ImpactStart = _.ImpactStart,
                    ImpactEnd = _.ImpactEnd
                })
                .ToArray();
            result.LastUpdateTime = lastUpdate;

            // query
            return new OkObjectResult(result);
        }

        private void AddBadRequest(HttpRequest req)
        {
            // add deny list
        }
    }
}
