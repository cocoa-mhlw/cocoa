using System.Threading.Tasks;
using Covid19Radar.Models;

namespace Covid19Radar.Services
{
    public interface IOtpService
    {
        Task SendAsync(OtpSendRequest request);
        Task<bool> ValidateAsync(OtpValidateRequest request);
    }
}
