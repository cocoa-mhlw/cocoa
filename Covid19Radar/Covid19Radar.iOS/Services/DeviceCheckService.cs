using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Covid19Radar.Services;
using Covid19Radar.iOS.Services;
using DeviceCheck;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Covid19Radar.Model;

[assembly: Dependency(typeof(DeviceCheckService))]
namespace Covid19Radar.iOS.Services
{
    public class DeviceCheckService : IDeviceVerifier
    {
        /// <summary>
        /// Verification device information required for positive submissions
        /// </summary>
        /// <returns>Device Verification Payload</returns>
		public async Task<string> VerifyAsync(SelfDiagnosisSubmission submission)
        {
            var token = await DeviceCheck.DCDevice.CurrentDevice.GenerateTokenAsync();
            return Convert.ToBase64String(token.ToArray());
        }
/*
        public async Task<string> GetDeviceVerificationPayload()
        {
            if (!DCDevice.CurrentDevice.Supported) return "";
            var token = await DCDevice.CurrentDevice.GenerateTokenAsync();
            return token.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
        }
*/
    }
}