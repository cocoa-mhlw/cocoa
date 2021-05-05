/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Android.Gms.SafetyNet;
using Covid19Radar.Droid.Services;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceCheckService))]
namespace Covid19Radar.Droid.Services
{
    public class DeviceCheckService : IDeviceVerifier
    {
        public Task<string> VerifyAsync(DiagnosisSubmissionParameter submission)
        {
            var nonce = GetNonce(submission);
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

        public static byte[] GetNonce(DiagnosisSubmissionParameter submission)
        {
            var cleartext = GetNonceClearText(submission);
            var nonce = GetSha256(cleartext);
            return nonce;

            static string GetNonceClearText(DiagnosisSubmissionParameter submission) =>
                string.Join("|", submission.AppPackageName, GetKeyString(submission.Keys), GetRegionString(submission.Regions), submission.VerificationPayload);

            static string GetKeyString(IEnumerable<DiagnosisSubmissionParameter.Key> keys) =>
                string.Join(",", keys.OrderBy(k => k.KeyData).Select(k => GetKeyStringCore(k)));

            static string GetKeyStringCore(DiagnosisSubmissionParameter.Key k) =>
                string.Join(".", k.KeyData, k.RollingStartNumber, k.RollingPeriod);

            static string GetRegionString(IEnumerable<string> regions) =>
                string.Join(",", regions.Select(r => r.ToUpperInvariant()).OrderBy(r => r));

            static byte[] GetSha256(string text)
            {
                using var sha = SHA256.Create();
                var textBytes = Encoding.UTF8.GetBytes(text);
                return sha.ComputeHash(textBytes);
            }
        }
    }
}
