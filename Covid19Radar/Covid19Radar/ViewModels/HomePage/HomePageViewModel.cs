using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;
using System.Globalization;

namespace Covid19Radar.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private readonly ExposureNotificationService exposureNotificationService;
        private UserDataModel userData;
        private string _startDate;
        private string _pastDate;

        public string StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }
        public string PastDate
        {
            get { return _pastDate; }
            set { SetProperty(ref _pastDate, value); }
        }

        public HomePageViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = AppResources.HomePageTitle;
            this.userDataService = userDataService;
            this.exposureNotificationService = exposureNotificationService;
            _ = exposureNotificationService.GetExposureNotificationConfig();
            _ = exposureNotificationService.StartExposureNotification();
            userData = this.userDataService.Get();
            StartDate = userData.StartDateTime.ToLocalTime().ToString("D", CultureInfo.CurrentCulture);

            TimeSpan timeSpan = DateTime.Now - userData.StartDateTime;
            PastDate = timeSpan.Days.ToString("D", CultureInfo.CurrentCulture);
            if (PastDate == "0")
            {
                PastDate = "";
            }
        }

        public Command OnClickHelp => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(HelpMenuPage), useModalNavigation: true);
        });

        public Command OnClickNotifyOther => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(NotifyOtherPage));
        });

        public Command OnClickExposures => new Command(async () =>
        {
            var count = exposureNotificationService.GetExposureCount();
            if (count > 0)
            {
                await NavigationService.NavigateAsync(nameof(ContactedNotifyPage));
                return;
            }
            await NavigationService.NavigateAsync(nameof(NotContactPage));
            return;
        });

        public Command OnClickShareApp => new Command(async () =>
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = AppConstants.AppStoreUrl,
                    Title = AppConstants.AppName
                });
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = AppConstants.GooglePlayUrl,
                    Title = AppConstants.AppName
                });
            }
        });
    }
}
