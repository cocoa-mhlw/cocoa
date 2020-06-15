using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{
    public interface IVerificationService
    {
        Task<bool> Verification(string payload);
    }
}
