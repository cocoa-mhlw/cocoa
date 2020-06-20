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
		public async Task<string> VerifyAsync(DiagnosisSubmissionParameter submission)
		{
			var token = await DeviceCheck.DCDevice.CurrentDevice.GenerateTokenAsync();
			return Convert.ToBase64String(token.ToArray());
		}

	}
}