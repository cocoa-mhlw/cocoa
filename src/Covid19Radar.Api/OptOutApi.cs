using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
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
        private readonly IValidationServerService ValidationServerService;

        public OptOutApi(
            IUserRepository userRepository,
            IDiagnosisRepository diagnosisRepository,
            IValidationUserService validation,
            IValidationServerService validationServerService,
            ILogger<OptOutApi> logger)
        {
            UserRepository = userRepository;
            DiagnosisRepository = diagnosisRepository;
            Validation = validation;
            Logger = logger;
            ValidationServerService = validationServerService;
        }

        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "OptOut/{userUuid}")] HttpRequest req,
            string userUuid)
        {
            Logger.LogInformation($"{nameof(OptOutApi)} processed a request.");

            // Check Valid Route
            IValidationServerService.ValidateResult validateResult = ValidationServerService.Validate(req);
            if (!validateResult.IsValid)
            {
                return validateResult.ErrorActionResult;
            }

            var user = new UserParameter() { UserUuid = userUuid };

            // validation
            var validationResult = await Validation.ValidateAsync(req, user);
            if (!validationResult.IsValid)
            {
                AddBadRequest(req);
                return validationResult.ErrorActionResult;
            }

            // delete tek
            await DiagnosisRepository.DeleteAsync(user);

            // NOTE:consider privacy: delete published Tek at after 14 days.

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
