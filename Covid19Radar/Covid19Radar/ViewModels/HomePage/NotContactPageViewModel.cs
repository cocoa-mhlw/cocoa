using Covid19Radar.Common;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class NotContactPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;

        public NotContactPageViewModel(INavigationService navigationService, ILoggerService loggerService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            this.loggerService = loggerService;
        }
        public Command OnClickShareApp => new Command(() =>
        {
            loggerService.StartMethod();

            AppUtils.PopUpShare();

            loggerService.EndMethod();
        });

    }
}
