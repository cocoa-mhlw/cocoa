/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using System;
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
        private readonly IUserDataRepository _userDataRepository;
        private readonly AbsExposureNotificationApiService exposureNotificationApiService;
        private readonly ILocalNotificationService localNotificationService;
        private readonly AbsExposureDetectionBackgroundService exposureDetectionBackgroundService;
        private readonly IDialogService dialogService;
        private readonly IExternalNavigationService externalNavigationService;

        private string _pastDate;
        public string PastDate
        {
            get { return _pastDate; }
            set { SetProperty(ref _pastDate, value); }
        }

        private string _latestConfirmationDate;
        public string LatestConfirmationDate
        {
            get { return _latestConfirmationDate; }
            set { SetProperty(ref _latestConfirmationDate, value); }
        }

        private bool _isVisibleENStatusActiveLayout;
        public bool IsVisibleENStatusActiveLayout
        {
            get { return _isVisibleENStatusActiveLayout; }
            set { SetProperty(ref _isVisibleENStatusActiveLayout, value); }
        }

        private bool _isVisibleENStatusUnconfirmedLayout;
        public bool IsVisibleENStatusUnconfirmedLayout
        {
            get { return _isVisibleENStatusUnconfirmedLayout; }
            set { SetProperty(ref _isVisibleENStatusUnconfirmedLayout, value); }
        }

        private bool _isVisibleENStatusStoppedLayout;
        public bool IsVisibleENStatusStoppedLayout
        {
            get { return _isVisibleENStatusStoppedLayout; }
            set { SetProperty(ref _isVisibleENStatusStoppedLayout, value); }
        }

        public HomePageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            ILocalNotificationService localNotificationService,
            AbsExposureDetectionBackgroundService exposureDetectionBackgroundService,
            IDialogService dialogService,
            IExternalNavigationService externalNavigationService
            ) : base(navigationService)
        {
            Title = AppResources.HomePageTitle;

            this.loggerService = loggerService;
            this._userDataRepository = userDataRepository;
            this.exposureNotificationApiService = exposureNotificationApiService;
            this.localNotificationService = localNotificationService;
            this.exposureDetectionBackgroundService = exposureDetectionBackgroundService;
            this.dialogService = dialogService;
            this.externalNavigationService = externalNavigationService;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            loggerService.StartMethod();

            // It seems the life cycle methods are not called after background fetch in iOS.
            // The days of use will be updated at this time.
            MessagingCenter.Unsubscribe<object>(this, AppConstants.IosOnActivatedMessage);
            MessagingCenter.Subscribe<object>(this, AppConstants.IosOnActivatedMessage, async (sender) =>
            {
                await UpdateView();
            });

            // Check Version
            AppUtils.CheckVersion(loggerService);

            await localNotificationService.PrepareAsync();

            await StartExposureNotificationAsync();

            _ = exposureDetectionBackgroundService.ExposureDetectionAsync();

            loggerService.EndMethod();
        }

        private async Task StartExposureNotificationAsync()
        {
            loggerService.StartMethod();

            try
            {
                _ = await exposureNotificationApiService.StartExposureNotificationAsync();
                await UpdateView();

            }
            catch (ENException exception)
            {
                loggerService.Exception("Failed to exposure notification start.", exception);
                await UpdateView();
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        public Command OnClickExposures => new Command(async () =>
        {
            loggerService.StartMethod();

            var dailySummaryList = await _userDataRepository.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay);
            var userExposureInformationList = _userDataRepository.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay) ?? new List<UserExposureInfo>();

            var count = dailySummaryList.Count() + userExposureInformationList.Count();

            await localNotificationService.DismissExposureNotificationAsync();

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

        public Command OnClickQuestionIcon => new Command(() =>
        {
            loggerService.StartMethod();

            UserDialogs.Instance.Alert(
                AppResources.LatestConfirmationDateExplanationDialogText,
                null,
                AppResources.ButtonClose);

            loggerService.EndMethod();
        });

        public Command OnClickCheckStopReason => new Command(async () =>
        {
            loggerService.StartMethod();

            var statusCodes = await exposureNotificationApiService.GetStatusCodesAsync();

            if (
            statusCodes.Contains(ExposureNotificationStatus.Code_Android.INACTIVATED)
            || statusCodes.Contains(ExposureNotificationStatus.Code_Android.FOCUS_LOST)
            || statusCodes.Contains(ExposureNotificationStatus.Code_iOS.Disabled)
            || statusCodes.Contains(ExposureNotificationStatus.Code_iOS.Unauthorized)
            )
            {
                bool isOK = await dialogService.ShowExposureNotificationOffWarningAsync();
                if (isOK)
                {
                    externalNavigationService.NavigateAppSettings();
                }
            }
            else if (
            statusCodes.Contains(ExposureNotificationStatus.Code_Android.BLUETOOTH_DISABLED)
            || statusCodes.Contains(ExposureNotificationStatus.Code_iOS.BluetoothOff)
            )
            {
                bool isOK = await dialogService.ShowBluetoothOffWarningAsync();
                if (isOK)
                {
                    externalNavigationService.NavigateBluetoothSettings();
                }
            }
            else if (
            statusCodes.Contains(ExposureNotificationStatus.Code_Android.LOCATION_DISABLED)
            )
            {
                bool isOK = await dialogService.ShowLocationOffWarningAsync();
                if (isOK)
                {
                    externalNavigationService.NavigateLocationSettings();
                }
            }

            loggerService.EndMethod();
        });

        private async Task UpdateView()
        {
            loggerService.StartMethod();

            var daysOfUse = _userDataRepository.GetDaysOfUse();

            PastDate = daysOfUse.ToString();

            var statusCodes = await exposureNotificationApiService.GetStatusCodesAsync();

            var isStopped =
                statusCodes.Contains(ExposureNotificationStatus.Code_Android.INACTIVATED)
                || statusCodes.Contains(ExposureNotificationStatus.Code_Android.FOCUS_LOST)
                || statusCodes.Contains(ExposureNotificationStatus.Code_iOS.Disabled)
                || statusCodes.Contains(ExposureNotificationStatus.Code_iOS.Unauthorized)
                || statusCodes.Contains(ExposureNotificationStatus.Code_Android.BLUETOOTH_DISABLED)
                || statusCodes.Contains(ExposureNotificationStatus.Code_iOS.BluetoothOff)
                || statusCodes.Contains(ExposureNotificationStatus.Code_Android.LOCATION_DISABLED);
            var canConfirmExposure = _userDataRepository.IsCanConfirmExposure();

            if (isStopped)
            {
                IsVisibleENStatusActiveLayout = false;
                IsVisibleENStatusUnconfirmedLayout = false;
                IsVisibleENStatusStoppedLayout = true;
            }
            else if (!canConfirmExposure)
            {
                IsVisibleENStatusActiveLayout = false;
                IsVisibleENStatusUnconfirmedLayout = true;
                IsVisibleENStatusStoppedLayout = false;
            }
            else
            {
                IsVisibleENStatusActiveLayout = true;
                IsVisibleENStatusUnconfirmedLayout = false;
                IsVisibleENStatusStoppedLayout = false;

                var latestUtcDate = _userDataRepository.GetLastConfirmedDate();
                if (latestUtcDate == null)
                {
                    LatestConfirmationDate = AppResources.InProgressText;
                }
                else
                {
                    try
                    {
                        var latestLocalDate = latestUtcDate.Value.ToLocalTime();
                        LatestConfirmationDate = latestLocalDate.ToString(AppResources.DateTimeFormatToDisplayOnHomePage);
                    }
                    catch (FormatException ex)
                    {
                        loggerService.Exception("Failed to conversion utc date time", ex);
                    }
                }
            }

            loggerService.EndMethod();
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await UpdateView();
        }

        public override async void OnResume()
        {
            base.OnResume();

            await StartExposureNotificationAsync();

            await UpdateView();
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

            await UpdateView();

            loggerService.EndMethod();
        }
    }
}
