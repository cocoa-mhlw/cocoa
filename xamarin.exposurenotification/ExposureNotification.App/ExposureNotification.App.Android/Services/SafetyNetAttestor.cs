using System.Threading.Tasks;
using Android.Gms.SafetyNet;
using ExposureNotification.App.Droid.Services;
using ExposureNotification.App.Services;
using ExposureNotification.Backend.Network;
using Xamarin.Forms;

[assembly: Dependency(typeof(SafetyNetAttestor))]

namespace ExposureNotification.App.Droid.Services
{
	partial class SafetyNetAttestor : IDeviceVerifier
	{
		public Task<string> VerifyAsync(SelfDiagnosisSubmission submission)
		{
			var nonce = submission.GetAndroidNonce();
			return GetSafetyNetAttestationAsync(nonce);
		}

		async Task<string> GetSafetyNetAttestationAsync(byte[] nonce)
		{
			using var client = SafetyNetClass.GetClient(Android.App.Application.Context);
			using var response = await client.AttestAsync(nonce, AppSettings.Instance.AndroidSafetyNetApiKey);
			return response.JwsResult;
		}
	}
}
