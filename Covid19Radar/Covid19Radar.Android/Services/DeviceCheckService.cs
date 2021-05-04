/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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

        public Task<string> VerifyAsync(Model.DiagnosisSubmissionParameter submission)
        {
            var nonce = submission.GetNonce();
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
