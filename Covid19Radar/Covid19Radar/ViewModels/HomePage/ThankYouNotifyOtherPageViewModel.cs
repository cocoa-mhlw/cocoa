using Covid19Radar.Common;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ThankYouNotifyOtherPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;

        public ThankYouNotifyOtherPageViewModel(INavigationService navigationService, ILoggerService loggerService) : base(navigationService)
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
