using System.Threading.Tasks;
using Covid19Radar.Model;

namespace Covid19Radar.Services
{
    public class OTPService
    {
        private readonly HttpDataService _httpDataService;

        public OTPService()
        {
            _httpDataService = Xamarin.Forms.DependencyService.Resolve<HttpDataService>();
        }

        public Task SendOTPAsync(UserDataModel user, string phoneNumber)
        {
            return _httpDataService.PostOTPAsync(user, phoneNumber);
        }
    }
}
