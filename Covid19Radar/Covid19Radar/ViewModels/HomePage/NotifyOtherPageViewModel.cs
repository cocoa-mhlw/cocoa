using System.Collections.Generic;
using System.Linq;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;
using System;
using System.Windows.Input;
using Prism.Navigation.Xaml;
using Acr.UserDialogs;
using Covid19Radar.Renderers;
using Covid19Radar.Views;
using Xamarin.Essentials;

namespace Covid19Radar.ViewModels
{
    public class NotifyOtherPageViewModel : ViewModelBase
    {
        public bool IsEnabled { get; set; } = true;
        public string DiagnosisUid { get; set; }
        public DateTime? DiagnosisTimestamp { get; set; } = DateTime.Now;

        public DateTime? SelectedDate { get; } = DateTime.Today;
        public DateTime? MaxDate { get;} = DateTime.Today.AddMonths(1);
        public DateTime? MinDate { get; } = DateTime.Today.AddMonths(-1);

        private readonly UserDataService userDataService;
        private UserDataModel userData;


        public NotifyOtherPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }

        public bool DiagnosisPending
    => (userData.LatestDiagnosis?.DiagnosisDate ?? DateTimeOffset.MinValue)
        >= DateTimeOffset.UtcNow.AddDays(-14);

        public DateTimeOffset DiagnosisShareTimestamp
            => userData.LatestDiagnosis?.DiagnosisDate ?? DateTimeOffset.MinValue;

        public Command SharePositiveDiagnosisCommand
            => new Command(() =>
            {
                // To Share Positive Diag
            });

        public Command LearnMoreCommand
            => new Command(() =>
            {
                // TODO move to browser
                Browser.OpenAsync("https://www.google.co.jp/");
            });



        public Command OnClickRegister => (new Command(async () =>
       {
           // Uploading EN File
           //await NavigationService.NavigateAsync("SharePositiveDiagnosisPage");
           // Sample Move to URL ( Open Browser)
           // await Xamarin.Essentials.Browser.OpenAsync(Resources.AppResources.NotifyOthersLearnMoreUrl);

       }));

        public Command OnClickAfter => (new Command(async () =>
        {
            var check = await UserDialogs.Instance.ConfirmAsync("あとで設定しますか?", "陽性登録", "後にする", "登録へ戻る");
            if (check)
            {
                await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
            }

        }));
    }
}
