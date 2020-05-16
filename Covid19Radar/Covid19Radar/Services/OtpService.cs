using System.Threading.Tasks;
using Covid19Radar.Model;

namespace Covid19Radar.Services
{
    public class OTPService
    {
        private readonly HttpDataService _httpDataService;

        public OTPService(HttpDataService httpDataService)
        {
            _httpDataService = httpDataService;
        }

        public Task SendOTPAsync(UserDataModel user, string phoneNumber)
        {
            return _httpDataService.PostOTPAsync(user, phoneNumber);
        }
    }
}
