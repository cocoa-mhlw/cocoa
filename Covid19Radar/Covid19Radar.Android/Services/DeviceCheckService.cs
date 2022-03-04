/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Android.Gms.SafetyNet;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Newtonsoft.Json.Linq;

namespace Covid19Radar.Droid.Services
{
    public class DeviceCheckService : IDeviceVerifier
    {
        public Task<string> VerifyAsync(DiagnosisSubmissionParameter submission)
        {
            var nonce = DeviceVerifierUtils.CreateAndroidNonceV3(submission);
            return GetSafetyNetAttestationAsync(nonce);
        }

        public Task<string> VerifyAsync(V1EventLogRequest eventLogRequest)
        {
            var nonce = DeviceVerifierUtils.CreateAndroidNonceV3(eventLogRequest);
            return GetSafetyNetAttestationAsync(nonce);
        }

        public bool IsErrorPayload(string jwsResult)
        {
            try
            {
                var jwt = new JwtSecurityToken(jwsResult);
                var payloadJson = jwt.Payload.SerializeToJson();

                var error = JObject.Parse(payloadJson).Value<string>("error");
                if (!string.IsNullOrEmpty(error))
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verification device information required for positive submissions
        /// </summary>
        /// <returns>Device Verification Payload</returns>
        private static async Task<string> GetSafetyNetAttestationAsync(byte[] nonce)
        {
            using var client = SafetyNetClass.GetClient(Android.App.Application.Context);
            using var response = await client.AttestAsync(nonce, AppSettings.Instance.AndroidSafetyNetApiKey);
            return response.JwsResult;
        }
    }
}
