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
using Covid19Radar.Extensions;

#nullable enable

namespace Covid19Radar.Api
{
    public class InfectionRegister
    {
        private readonly IInfectionProcessRepository InfectionProcessRepository;
        private readonly IInfectionService Infection;
        private readonly IValidationUserService Validation;
        private readonly ILogger<InfectionRegister> Logger;

        public InfectionRegister(
            IInfectionProcessRepository infectionProcessRepository,
            IInfectionService infection,
            IValidationUserService validation,
            ILogger<InfectionRegister> logger)
        {
            InfectionProcessRepository = infectionProcessRepository;
            Infection = infection;
            Validation = validation;
            Logger = logger;
        }

        [FunctionName(nameof(InfectionRegister))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "Infection/Register")] HttpRequest req)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            // convert Postdata to InfectionRegisterParameter
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var param = JsonConvert.DeserializeObject<InfectionRegisterParameter>(requestBody);

            // validation
            var validationResult = await Validation.ValidateAsync(req, param);
            if (!validationResult.IsValid)
            {
                AddBadRequest(req);
                return validationResult.ErrorActionResult;
            }

            // Register
            var model = new InfectionProcessModel()
            {
                id = param.ProcessingNumber,
                PartitionKey = param.UserUuid,
                UserUuid = param.UserUuid,
                Major = param.Major,
                Minor = param.Minor,
                ProcessingNumber = param.ProcessingNumber
            };
            await InfectionProcessRepository.Upsert(model);
            return new StatusCodeResult(201);
        }

        private void AddBadRequest(HttpRequest req)
        {
            // add deny list
        }
    }
}
