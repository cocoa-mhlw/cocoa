using System.Threading.Tasks;
using ExposureNotification.Backend.Network;

namespace ExposureNotification.App.Services
{
	public interface IDeviceVerifier
	{
		Task<string> VerifyAsync(SelfDiagnosisSubmission submission);
	}
}
