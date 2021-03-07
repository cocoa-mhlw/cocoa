using System.Threading.Tasks;
using Covid19Radar.Services;
using Covid19Radar.Model;
using Android.Gms.SafetyNet;

namespace Covid19Radar.Droid.Services
{
    public class DeviceCheckService : IDeviceVerifier
    {

        public Task<string> VerifyAsync(DiagnosisSubmissionParameter submission)
        {
            var nonce = submission.GetAndroidNonce();
            return GetSafetyNetAttestationAsync(nonce);
        }

        /// <summary>
        /// Verification device information required for positive submissions
        /// </summary>
        /// <returns>Device Verification Payload</returns>
        async Task<string> GetSafetyNetAttestationAsync(byte[] nonce)
        {
            using var client = SafetyNetClass.GetClient(Android.App.Application.Context);
            using var response = await client.AttestAsync(nonce, AppSettings.Instance.AndroidSafetyNetApiKey);
            return response.JwsResult;
        }
    }
}