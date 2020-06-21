using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{
    public interface IVerificationService
    {
        Task<int> VerificationAsync(string payload);
    }
}
