using System;
using System.IO;
using System.Threading.Tasks;
using ExposureNotification.Backend.Database;
using ExposureNotification.Backend.DeviceVerification;
using ExposureNotification.Backend.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ExposureNotification.Backend.Functions
{
	public class SelfDiagnosis
	{
		readonly ExposureNotificationStorage storage;
		readonly IOptions<Settings> settings;

		public SelfDiagnosis(ExposureNotificationStorage storage, IOptions<Settings> settings)
		{
			this.storage = storage;
			this.settings = settings;
		}

		[FunctionName("UploadSelfDiagnosis")]
		public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "selfdiagnosis")] HttpRequest req, ILogger log)
		{
			var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

			if (req.Method.Equals("put", StringComparison.OrdinalIgnoreCase))
			{
				var diagnosis = JsonConvert.DeserializeObject<SelfDiagnosisSubmission>(requestBody);

				// Verification may be disabled for testing
				if (!settings.Value.DisableDeviceVerification)
				{
					var platform = AuthorizedAppConfig.ParsePlatform(diagnosis.Platform);
					var authApp = storage.GetAuthorizedApp(platform);

					// Verify the device payload (safetynet attestation on android, or device check token on iOS)
					if (!await Verify.VerifyDevice(diagnosis, DateTimeOffset.UtcNow, platform, authApp))
					{
						log.LogInformation($"Device Failed {platform} Attestation/Verification, returning OK");
						// The suggestion from Apple/Google is to return OK here to prevent abuse
						return new OkResult();
					}
				}

				if (!diagnosis.Validate())
				{
					log.LogInformation("Invalid Submission Key data - Validate() failed");
					return new OkResult();
				}

				try
				{
					await storage.SubmitPositiveDiagnosisAsync(diagnosis);
				}
				catch (InvalidOperationException)
				{
					log.LogInformation("Maximum keys for VerificationPayload reached, skipping key submission...");
				}
			}

			return new OkResult();
		}
	}
}
