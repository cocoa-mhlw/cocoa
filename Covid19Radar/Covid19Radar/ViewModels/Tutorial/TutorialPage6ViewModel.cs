using Covid19Radar.Model;
using Prism.Navigation;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using System.Threading.Tasks;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage6ViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IUserDataService userDataService;
        private UserDataModel userData;

        public TutorialPage6ViewModel(INavigationService navigationService, ILoggerService loggerService, IUserDataService userDataService) : base(navigationService)
        {
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            loggerService.StartMethod();

            await TutorialCompleteAsync();
            base.Initialize(parameters);

            loggerService.EndMethod();
        }
        private async Task TutorialCompleteAsync()
        {
            userData.IsPolicyAccepted = true;
            await userDataService.SetAsync(userData);
            loggerService.Info($"IsPolicyAccepted set to {userData.IsPolicyAccepted}");
        }
    }
}