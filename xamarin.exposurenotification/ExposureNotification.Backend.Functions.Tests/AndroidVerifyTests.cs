using System;
using System.IO;
using System.Threading.Tasks;
using ExposureNotification.Backend.Database;
using ExposureNotification.Backend.DeviceVerification;
using ExposureNotification.Backend.Network;
using Newtonsoft.Json;
using Xunit;

namespace ExposureNotification.Backend.Functions.Tests
{
	public class AndroidVerifyTests
	{
		static readonly SelfDiagnosisSubmission actualSubmission = 
			JsonConvert.DeserializeObject<SelfDiagnosisSubmission>(File.ReadAllText("TestAssets/submission.json"));

		const string sha256 = "pnpQ6KkSqZ+y0yJIxKc9eqNXIM8olKZ5/rV0mxvIDWg=";

		static readonly SelfDiagnosisSubmission submission = new SelfDiagnosisSubmission
		{
			AppPackageName = "com.companyname.ExposureNotification.app",
			Platform = "android",
			Regions = new[] { "default" },
			VerificationPayload = "POSITIVE_TEST_123456",
			Keys =
			{
				new ExposureKey { Key = "Qm8HISNU/bdI+9o1/4ZHZw==", RollingStart = 2650127, RollingDuration = 0, TransmissionRisk = 6 },
				new ExposureKey { Key = "A8sh1Z5hB7hFKejbzwclnA==", RollingStart = 2649983, RollingDuration = 3, TransmissionRisk = 2 },
			}
		};

		[Fact]
		public void VerifyNonceTest()
		{
			var nonce = submission.GetAndroidNonce();

			Assert.Equal(sha256, Convert.ToBase64String(nonce));
		}

		[Fact]
		public void ParsePayloadTest()
		{
			var claims = AndroidVerify.ParsePayload(actualSubmission.DeviceVerificationPayload);
			var nonce = submission.GetAndroidNonce();

			Assert.Equal(Convert.ToBase64String(nonce), Convert.ToBase64String(claims.Nonce));
		}

		[Fact]
		public async Task VerifyTokenTest()
		{
			var app = new AuthorizedAppConfig
			{
				PackageName = "com.xamarin.exposurenotification.sampleapp",
				Platform = "android",
			};

			var nonce = submission.GetAndroidNonce();

			var verified = await AndroidVerify.VerifyToken(actualSubmission.DeviceVerificationPayload, nonce, DateTimeOffset.UtcNow, app);

			Assert.True(verified);
		}
	}
}
