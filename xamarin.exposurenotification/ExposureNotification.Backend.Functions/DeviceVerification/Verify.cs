using System;
using System.Threading.Tasks;
using ExposureNotification.Backend.Database;
using ExposureNotification.Backend.Network;

namespace ExposureNotification.Backend.DeviceVerification
{
	static class Verify
	{
		public static Task<bool> VerifyDevice(SelfDiagnosisSubmission submission, DateTimeOffset requestTime, AuthorizedAppConfig.DevicePlatform platform, AuthorizedAppConfig auth) =>
			platform switch
			{
				AuthorizedAppConfig.DevicePlatform.Android => AndroidVerify.VerifyToken(submission.DeviceVerificationPayload, submission.GetAndroidNonce(), requestTime, auth),
				AuthorizedAppConfig.DevicePlatform.iOS => AppleVerify.VerifyToken(submission.DeviceVerificationPayload, requestTime, auth),
				_ => Task.FromResult(false),
			};
	}
}
