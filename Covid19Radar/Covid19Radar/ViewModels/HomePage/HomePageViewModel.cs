using System;
using System.Diagnostics;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IUserDataService userDataService;
        private readonly IExposureNotificationService exposureNotificationService;
        private readonly IApplicationPropertyService applicationPropertyService;

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

        public HomePageViewModel(INavigationService navigationService, ILoggerService loggerService, IUserDataService userDataService, IExposureNotificationService exposureNotificationService, IApplicationPropertyService applicationPropertyService) : base(navigationService)
        {
            Title = AppResources.HomePageTitle;
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            this.exposureNotificationService = exposureNotificationService;
            this.applicationPropertyService = applicationPropertyService;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            loggerService.StartMethod();

            { // This block is for test use only
                var testUserData = new UserDataModel();
                testUserData.StartDateTime = DateTime.Now;
                var testUserDataAsJsonString = Utils.SerializeToJson(testUserData);
                await applicationPropertyService.SavePropertiesAsync("UserData", testUserDataAsJsonString);

                var userDataFromApplicationPropertyAsJsonString = applicationPropertyService.GetProperties("UserData").ToString();
                var userDataFromApplicationProperty = Utils.DeserializeFromJson<UserDataModel>(userDataFromApplicationPropertyAsJsonString);

                if(testUserData.StartDateTime.Equals(userDataFromApplicationProperty.StartDateTime))
                {
                    loggerService.Debug("testUserData.StartDateTime == userDataFromApplicationProperty.StartDateTime," +
                        $" {userDataFromApplicationProperty.StartDateTime}");
                }
            }

            var startDate = userDataService.GetStartDate();
            StartDate = startDate.ToLocalTime().ToString("D");

            var daysOfUse = userDataService.GetDaysOfUse();
            PastDate = daysOfUse.ToString();


            // Check Version
            AppUtils.CheckVersion(loggerService);
            try
            {
                await exposureNotificationService.StartExposureNotification();
                await exposureNotificationService.FetchExposureKeyAsync();

                var statusMessage = await exposureNotificationService.UpdateStatusMessageAsync();
                loggerService.Info($"Exposure notification status: {statusMessage}");

                base.Initialize(parameters);

                loggerService.EndMethod();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                loggerService.Exception("Failed to exposure notification status.", ex);
                loggerService.EndMethod();
            }
        }

        public Command OnClickExposures => new Command(async () =>
        {
            loggerService.StartMethod();

            var count = exposureNotificationService.GetExposureCount();
            loggerService.Info($"Exposure count: {count}");
            if (count > 0)
            {
                await NavigationService.NavigateAsync(nameof(ContactedNotifyPage));
                loggerService.EndMethod();
                return;
            }
            else
            {
                await NavigationService.NavigateAsync(nameof(NotContactPage));
                loggerService.EndMethod();
                return;
            }
        });

        public Command OnClickShareApp => new Command(() =>
       {
           loggerService.StartMethod();

           AppUtils.PopUpShare();

           loggerService.EndMethod();
       });
    }
}
