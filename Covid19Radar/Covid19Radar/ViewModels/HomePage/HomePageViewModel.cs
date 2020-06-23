using System;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

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

            userData = this.userDataService.Get();
            StartDate = userData.GetLocalDateString();

            TimeSpan timeSpan = DateTime.Now - userData.StartDateTime;
            PastDate = timeSpan.Days.ToString();
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            try
            {
                await exposureNotificationService.StartExposureNotification();
                await exposureNotificationService.FetchExposureKeyAsync();
                base.Initialize(parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

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

        public Command OnClickShareApp => new Command(() =>
       {
           AppUtils.PopUpShare();
       });
    }
}
