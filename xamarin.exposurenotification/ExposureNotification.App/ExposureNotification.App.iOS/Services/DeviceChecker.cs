using System;
using System.Threading.Tasks;
using ExposureNotification.App.iOS.Services;
using ExposureNotification.App.Services;
using ExposureNotification.Backend.Network;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceChecker))]

namespace ExposureNotification.App.iOS.Services
{
	public class DeviceChecker : IDeviceVerifier
	{
		public async Task<string> VerifyAsync(SelfDiagnosisSubmission submission)
		{
			var token = await DeviceCheck.DCDevice.CurrentDevice.GenerateTokenAsync();
			return Convert.ToBase64String(token.ToArray());
		}
	}
}