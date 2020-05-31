using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExposureNotification.Backend.Database;
using ExposureNotification.Backend.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ExposureNotification.Backend.Functions
{
	public class CreateDummyKeys
	{
		readonly ExposureNotificationStorage storage;
		readonly Random random;

		public CreateDummyKeys(ExposureNotificationStorage storage)
		{
			this.storage = storage;

			random = new Random();
		}

#if DEBUG
		[FunctionName("CreateDummyKeys")]
#endif
		public async Task Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "dev/dummy-keys")] HttpRequest req, ILogger logger)
		{
			logger.LogInformation("Adding dummy keys...");

			var diagnosisUid = random.Next(100001, 128000).ToString();

			await storage.AddDiagnosisUidsAsync(new[] { diagnosisUid });

			var submission = new SelfDiagnosisSubmission(true)
			{
				VerificationPayload = diagnosisUid,
				Regions = PickRandom(new[] { "ca" }, new[] { "za" }, new[] { "ca", "za" }),
				AppPackageName = "com.xamarin.exposurenotification.sampleapp",
				Platform = PickRandom("android", "ios"),
				Keys = GetKeys()
			};

			await storage.SubmitPositiveDiagnosisAsync(submission);
		}

		T PickRandom<T>(params T[] items)
		{
			var i = random.Next(items.Length);
			return items[i];
		}

		List<ExposureKey> GetKeys()
		{
			var now = DateTimeOffset.UtcNow;

			var keys = new List<ExposureKey>();
			for (var i = 1; i <= 14; i++)
			{
				var buffer = new byte[16];
				random.NextBytes(buffer);

				var day = now.AddDays(-1 * i);
				var dt = new DateTimeOffset(day.Year, day.Month, day.Day, 0, 0, 0, 0, TimeSpan.Zero);
				keys.Add(new ExposureKey
				{
					Key = Convert.ToBase64String(buffer),
					RollingStart = dt.ToUnixTimeSeconds(),
					RollingDuration = 144,
					TransmissionRisk = random.Next(1, 8)
				});
			}
			return keys;
		}
	}
}
