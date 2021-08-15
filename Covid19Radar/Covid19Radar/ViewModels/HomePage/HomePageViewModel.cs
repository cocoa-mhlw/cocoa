/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HomePageViewModel : ViewModelBase, IExposureNotificationEventCallback
    {
        private readonly ILoggerService loggerService;
        private readonly IUserDataService userDataService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly AbsExposureNotificationApiService exposureNotificationApiService;
        private readonly ILocalNotificationService localNotificationService;

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

        public HomePageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDataService userDataService,
            IUserDataRepository userDataRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            ILocalNotificationService localNotificationService
            ) : base(navigationService)
        {
            Title = AppResources.HomePageTitle;
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            this._userDataRepository = userDataRepository;
            this.exposureNotificationApiService = exposureNotificationApiService;
            this.localNotificationService = localNotificationService;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            loggerService.StartMethod();

            // It seems the life cycle methods are not called after background fetch in iOS.
            // The days of use will be updated at this time.
            MessagingCenter.Unsubscribe<object>(this, AppConstants.IosOnActivatedMessage);
            MessagingCenter.Subscribe<object>(this, AppConstants.IosOnActivatedMessage, (sender) =>
            {
                SettingDaysOfUse();
            });

            var startDate = userDataService.GetStartDate();
            StartDate = startDate.ToLocalTime().ToString("D");

            SettingDaysOfUse();

            // Check Version
            AppUtils.CheckVersion(loggerService);

            await StartExposureNotificationAsync();

            await localNotificationService.PrepareAsync();

            // TODO We should replace below line with new FetchExposureKeyAsync method(with Cappuccino).
            // await exposureNotificationService.FetchExposureKeyAsync();
        }

        private async Task StartExposureNotificationAsync()
        {
            loggerService.StartMethod();

            try
            {
                _ = await exposureNotificationApiService.StartExposureNotificationAsync();
                await UpdateStatusesAsync();
            }
            catch (ENException exception)
            {
                loggerService.Exception("Failed to exposure notification start.", exception);
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        private async Task UpdateStatusesAsync()
        {
            loggerService.StartMethod();

            try
            {
                IList<ExposureNotificationStatus> statuses = await exposureNotificationApiService.GetStatusesAsync();
                var (alertMessageList, messageList) = GetStatusMessage(statuses);

                string alertMessgage = string.Join("\n", alertMessageList.Where(str => str != null));
                if (!string.IsNullOrEmpty(alertMessgage))
                {
                    await UserDialogs.Instance.AlertAsync(
                        alertMessgage,
                        "", AppResources.ButtonOk
                        );
                }

                string statusMessage = string.Join("\n", messageList.Where(str => str != null));
                loggerService.Info($"Exposure notification status: {statusMessage}");
            }
            catch (ENException exception)
            {
                loggerService.Exception("Failed to exposure notification status.", exception);
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        private (IList<string>?, IList<string>?) GetStatusMessage(IList<ExposureNotificationStatus> statuses)
        {
            IList<string?> alertMessageList = new List<string?>();
            IList<string?> messageList = new List<string?>();

            foreach (var status in statuses)
            {
                var (alertMessage, message) = ConvertToMessage(status);
                alertMessageList.Add(alertMessage);
                messageList.Add(message);
            }

            return (alertMessageList, messageList);
        }

        private (string?, string?) ConvertToMessage(ExposureNotificationStatus status)
        {
            string? alertMessage = null;
            string? message = null;

            switch (status.Code)
            {
                case ExposureNotificationStatus.Code_Android.INACTIVATED:
                case ExposureNotificationStatus.Code_iOS.Disabled:
                    alertMessage = AppResources.ExposureNotificationStatusMessageDisabled;
                    message = AppResources.ExposureNotificationStatusMessageDisabled;
                    break;
                case ExposureNotificationStatus.Code_Android.ACTIVATED:
                case ExposureNotificationStatus.Code_iOS.Active:
                    alertMessage = null;
                    message = AppResources.ExposureNotificationStatusMessageActive;
                    break;
                case ExposureNotificationStatus.Code_Android.BLUETOOTH_DISABLED:
                case ExposureNotificationStatus.Code_iOS.BluetoothOff:
                    // call out settings in each os
                    alertMessage = AppResources.ExposureNotificationStatusMessageBluetoothOff;
                    message = AppResources.ExposureNotificationStatusMessageBluetoothOff;
                    break;
                case ExposureNotificationStatus.Code_Android.NO_CONSENT:
                case ExposureNotificationStatus.Code_iOS.Restricted:
                    // call out settings in each os
                    alertMessage = AppResources.ExposureNotificationStatusMessageRestricted;
                    message = AppResources.ExposureNotificationStatusMessageRestricted;
                    break;
                case ExposureNotificationStatus.Code_Android.UNKNOWN:
                case ExposureNotificationStatus.Code_iOS.Unknown:
                    alertMessage = AppResources.ExposureNotificationStatusMessageUnknown;
                    message = AppResources.ExposureNotificationStatusMessageUnknown;
                    break;
                default:
                    break;
            }

            return (alertMessage, message);
        }

        public Command OnClickExposures => new Command(async () =>
        {
            loggerService.StartMethod();

            var (dailySummaryList, userExposureWindowList) = await _userDataRepository.GetExposureWindowDataAsync(AppConstants.DaysOfExposureInformationToDisplay);
            var (_, userExposureInformationList) = await _userDataRepository.GetUserExposureDataAsync(AppConstants.DaysOfExposureInformationToDisplay);

            var count = dailySummaryList.Count() + userExposureInformationList.Count();

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

        private void SettingDaysOfUse()
        {
            loggerService.StartMethod();
            var daysOfUse = userDataService.GetDaysOfUse();
            PastDate = daysOfUse.ToString();
            loggerService.EndMethod();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            SettingDaysOfUse();
        }

        public override async void OnResume()
        {
            base.OnResume();

            await StartExposureNotificationAsync();

            SettingDaysOfUse();
        }

        public async void OnEnabled()
        {
            loggerService.StartMethod();

            await StartExposureNotificationAsync();

            loggerService.EndMethod();
        }

        public async void OnDeclined()
        {
            loggerService.StartMethod();

            await UpdateStatusesAsync();

            loggerService.EndMethod();
        }
    }
}
