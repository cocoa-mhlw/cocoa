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

[assembly: Dependency(typeof(DeviceCheckService))]
namespace Covid19Radar.Droid.Services
{
    public class DeviceCheckService : IDeviceCheckService
    {
        /// <summary>
        /// Verification device information required for positive submissions
        /// </summary>
        /// <returns>Device Verification Payload</returns>
        public Task<string> GetDeviceVerificationPayload()
        {
            return Task.FromResult("");
        }
    }
}