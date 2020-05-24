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
        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public NotifyOtherPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            SelectedDate = DateTime.Today;
            MaxDate = DateTime.Today.AddMonths(1);
            MinDate = DateTime.Today.AddMonths(-1);
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
            this.userDataService.UserDataChanged += _userDataChanged;
        }

        private void _userDataChanged(object sender, UserDataModel e)
        {
            userData = this.userDataService.Get();
        }


        private string _notifyCode;
        public string NotifyCode
        {
            get { return _notifyCode; }
            set { SetProperty(ref _notifyCode, value); }
        }


        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }

        private DateTime _maxDate;
        public DateTime MaxDate
        {
            get { return _maxDate; }
            set { SetProperty(ref _maxDate, value); }
        }

        private DateTime _minDate;
        public DateTime MinDate
        {
            get { return _minDate; }
            set { SetProperty(ref _minDate, value); }
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
