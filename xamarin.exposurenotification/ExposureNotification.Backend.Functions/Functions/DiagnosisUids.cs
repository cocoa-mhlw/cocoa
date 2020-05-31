using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ExposureNotification.Backend.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace ExposureNotification.Backend.Functions
{
	public class DiagnosisUids
	{
		readonly ExposureNotificationStorage storage;

		public DiagnosisUids(ExposureNotificationStorage storage)
		{
			this.storage = storage;
		}

		[FunctionName("ManageDiagnosisUids")]
		public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", "delete", Route = "manage/diagnosis-uids")] HttpRequest req)
		{
			var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

			if (req.Method.Equals("put", StringComparison.OrdinalIgnoreCase))
			{
				var diagnosisUids = JsonConvert.DeserializeObject<IEnumerable<string>>(requestBody);

				await storage.AddDiagnosisUidsAsync(diagnosisUids);
			}
			else if (req.Method.Equals("delete", StringComparison.OrdinalIgnoreCase))
			{
				var diagnosisUids = JsonConvert.DeserializeObject<IEnumerable<string>>(requestBody);

				await storage.RemoveDiagnosisUidsAsync(diagnosisUids);
			}

			return new OkResult();
		}
	}
}
