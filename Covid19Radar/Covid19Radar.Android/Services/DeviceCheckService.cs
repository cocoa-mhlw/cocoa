using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Covid19Radar.Services;
using Covid19Radar.Droid.Services;
using Xamarin.Forms;
using Covid19Radar.Model;
using Covid19Radar.Common;
using Android.Gms.SafetyNet;

[assembly: Dependency(typeof(DeviceCheckService))]
namespace Covid19Radar.Droid.Services
{
    public class DeviceCheckService : IDeviceVerifier
    {

        readonly string safetyNetApiKey = "YOUR-KEY-HERE";

        public Task<string> VerifyAsync(SelfDiagnosisSubmission submission)
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
            using var response = await client.AttestAsync(nonce, safetyNetApiKey);
            return response.JwsResult;
        }
    }
}