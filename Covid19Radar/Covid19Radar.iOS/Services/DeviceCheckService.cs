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

[assembly: Dependency(typeof(DeviceCheckService))]
namespace Covid19Radar.iOS.Services
{
    public class DeviceCheckService : IDeviceCheckService
    {
        /// <summary>
        /// Verification device information required for positive submissions
        /// </summary>
        /// <returns>Device Verification Payload</returns>
        public async Task<string> GetDeviceVerificationPayload()
        {
            if (!DCDevice.CurrentDevice.Supported) return "";
            var token = await DCDevice.CurrentDevice.GenerateTokenAsync();
            return token.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
        }
    }
}