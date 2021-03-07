using System;
using System.Threading.Tasks;
using Covid19Radar.Services;
using Covid19Radar.Model;

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