using System;
using System.Threading.Tasks;
using ExposureNotification.Backend.Database;
using ExposureNotification.Backend.Network;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Xunit;

namespace ExposureNotification.Backend.Functions.Tests
{
	public class ExposureNotificationStorageTests
	{
		public ExposureNotificationStorageTests()
		{
			var builder = new DbContextOptionsBuilder()
				.UseInMemoryDatabase("ExposureNotificationStorageTests")
				.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
			var context = new ExposureNotificationContext(builder.Options);

			var settings = new Settings();
			var options = Options.Create(settings);
			
			Storage = new ExposureNotificationStorage(context, options);
		}

		public ExposureNotificationStorage Storage { get; }

		[Fact]
		public async Task Add_Keys_Test()
		{
			var diagnosisUids = new[] { "adduid1", "adduid2", "adduid3" };

			await Storage.AddDiagnosisUidsAsync(diagnosisUids);

			foreach (var d in diagnosisUids)
				Assert.True(await Storage.CheckIfDiagnosisUidExistsAsync(d));
		}

		[Fact]
		public async Task Add_Duplicate_Keys_Test()
		{
			var diagnosisUids = new[] { "dupadduid1", "dupadduid2", "dupadduid3" };

			await Storage.AddDiagnosisUidsAsync(diagnosisUids);

			await Storage.AddDiagnosisUidsAsync(diagnosisUids);

			foreach (var d in diagnosisUids)
				Assert.True(await Storage.CheckIfDiagnosisUidExistsAsync(d));
		}

		[Fact]
		public async Task Remove_Keys_Test()
		{
			var diagnosisUids = new[] { "rmuid1", "rmuid2", "rmuid3" };

			await Storage.AddDiagnosisUidsAsync(diagnosisUids);

			foreach (var d in diagnosisUids)
				Assert.True(await Storage.CheckIfDiagnosisUidExistsAsync(d));

			await Storage.RemoveDiagnosisUidsAsync(diagnosisUids);

			foreach (var d in diagnosisUids)
				Assert.False(await Storage.CheckIfDiagnosisUidExistsAsync(d));
		}

		//[Fact]
		//public async Task Submit_Diagnosis_Test()
		//{
		//	var keys = Utils.GenerateTemporaryExposureKeys(14);

		//	await Storage.AddDiagnosisUidsAsync(new[] { "posuid1" });

		//	await Storage.SubmitPositiveDiagnosisAsync(new ExposureNotificationStorage.SelfDiagnosisSubmissionRequest
		//	{
		//		DiagnosisUid = "posuid1",
		//		Keys = keys
		//	});

		//	var allKeys = await Storage.GetAllKeysAsync();

		//	var keyToEnsureExists = keys.Skip(keys.Count / 2).First();

		//	Assert.Contains(allKeys, p => p.KeyData.SequenceEqual(keyToEnsureExists.KeyData));
		//}

		[Fact]
		public async Task Submit_Diagnosis_Fails_Test()
		{
			var keys = Utils.GenerateExposureKeys(14);

			await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			{
				await Storage.SubmitPositiveDiagnosisAsync(new SelfDiagnosisSubmission
				{
					VerificationPayload = "notaddeduid1",
					Keys = keys
				});
			});
		}

		//[Fact]
		//public async Task Page_Keys_Test()
		//{
		//	var keys = Utils.GenerateTemporaryExposureKeys(1);

		//	var expectedCount = keys.Count();

		//	Storage.DeleteAllKeysAsync();

		//	await Storage.AddDiagnosisUidsAsync(new[] { "testkeys" });

		//	await Storage.SubmitPositiveDiagnosisAsync(
		//		new ExposureNotificationStorage.SelfDiagnosisSubmissionRequest
		//		{
		//			DiagnosisUid = "testkeys",
		//			Keys = keys
		//		});

		//	var allKeys = await Storage.GetAllKeysAsync();

		//	Assert.Equal(expectedCount, allKeys.Count);
		//}
	}
}
