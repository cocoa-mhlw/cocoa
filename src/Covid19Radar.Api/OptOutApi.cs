using Covid19Radar.DataAccess;
using Covid19Radar.Models;
using Covid19Radar.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Covid19Radar.Api
{
    public class OptOutApi
    {
        private readonly IUserRepository UserRepository;
        private readonly IDiagnosisRepository DiagnosisRepository;
        private readonly IValidationUserService Validation;
        private readonly ILogger<OptOutApi> Logger;

        public OptOutApi(
            IUserRepository userRepository,
            IDiagnosisRepository diagnosisRepository,
            IValidationUserService validation,
            ILogger<OptOutApi> logger)
        {
            UserRepository = userRepository;
            DiagnosisRepository = diagnosisRepository;
            Validation = validation;
            Logger = logger;
        }

        [FunctionName(nameof(OptOutApi))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "OptOut/{userUuid}")] HttpRequest req,
            string userUuid)
        {
            Logger.LogInformation($"{nameof(OptOutApi)} processed a request.");

            var user = new UserParameter() { UserUuid = userUuid };

            // validation
            var validationResult = await Validation.ValidateAsync(req, user);
            if (!validationResult.IsValid)
            {
                AddBadRequest(req);
                return validationResult.ErrorActionResult;
            }

            // delete tek
            await DiagnosisRepository.Delete(user);

            // delete user
            await UserRepository.Delete(user);

            return new NoContentResult();
        }

        private void AddBadRequest(HttpRequest req)
        {
            // add deny list
        }

    }
}
