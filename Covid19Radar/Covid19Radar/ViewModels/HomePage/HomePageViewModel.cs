/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Chino;
using Covid19Radar.Common;
using System;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Covid19Radar.ViewModels
{
    public class HomePageViewModel : ViewModelBase, IExposureNotificationEventCallback
    {
        public string SharingThisAppReadText => $"{AppResources.HomePageDescription5} {AppResources.Button}";
        public string LocalNotificationOffReadText => $"{AppResources.HomePageLocalNotificationOffWarningLabelText} {AppResources.Button}";

        private readonly ILoggerService loggerService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly IExposureRiskCalculationService _exposureRiskCalculationService;
        private readonly AbsExposureNotificationApiService exposureNotificationApiService;
        private readonly ILocalNotificationService localNotificationService;
        private readonly AbsExposureDetectionBackgroundService exposureDetectionBackgroundService;
        private readonly ICheckVersionService checkVersionService;
        private readonly IEssentialsService essentialsService;
        private readonly IDialogService dialogService;
        private readonly IExternalNavigationService externalNavigationService;

        private readonly IExposureConfigurationRepository exposureConfigurationRepository;
        private readonly IExposureRiskCalculationConfigurationRepository exposureRiskCalculationConfigurationRepository;

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

        private bool _isVisibleLocalNotificationOffWarningLayout;
        public bool IsVisibleLocalNotificationOffWarningLayout
        {
            get { return _isVisibleLocalNotificationOffWarningLayout; }
            set { SetProperty(ref _isVisibleLocalNotificationOffWarningLayout, value); }
        }

        public HomePageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IExposureDataRepository exposureDataRepository,
            IExposureRiskCalculationService exposureRiskCalculationService,
            AbsExposureNotificationApiService exposureNotificationApiService,
            ILocalNotificationService localNotificationService,
            AbsExposureDetectionBackgroundService exposureDetectionBackgroundService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            IExposureRiskCalculationConfigurationRepository exposureRiskCalculationConfigurationRepository,
            ICheckVersionService checkVersionService,
            IEssentialsService essentialsService,
            IDialogService dialogService,
            IExternalNavigationService externalNavigationService
            ) : base(navigationService)
        {
            Title = AppResources.HomePageTitle;

            this.loggerService = loggerService;
            this._userDataRepository = userDataRepository;
            this._exposureDataRepository = exposureDataRepository;
            this._exposureRiskCalculationService = exposureRiskCalculationService;
            this.exposureNotificationApiService = exposureNotificationApiService;
            this.localNotificationService = localNotificationService;
            this.exposureDetectionBackgroundService = exposureDetectionBackgroundService;
            this.exposureConfigurationRepository = exposureConfigurationRepository;
            this.exposureRiskCalculationConfigurationRepository = exposureRiskCalculationConfigurationRepository;
            this.checkVersionService = checkVersionService;
            this.essentialsService = essentialsService;
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
            _ = Task.Run(async () => {
                bool isUpdated = await checkVersionService.IsUpdateVersionExistAsync();
                if (isUpdated)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await UserDialogs.Instance.AlertAsync(
                            AppResources.AppUtilsGetNewVersionDescription,
                            AppResources.AppUtilsGetNewVersionTitle,
                            AppResources.ButtonOk
                            );
                        await Browser.OpenAsync(essentialsService.StoreUrl, BrowserLaunchMode.External);
                    });
                }
            });

            // Load necessary files asynchronous
            _ = exposureConfigurationRepository.GetExposureConfigurationAsync();
            _ = exposureRiskCalculationConfigurationRepository
                .GetExposureRiskCalculationConfigurationAsync(preferCache: false);

            await localNotificationService.PrepareAsync();

            await StartExposureNotificationAsync();

            _ = Task.Run(async () => {
                try
                {
                    await exposureDetectionBackgroundService.ExposureDetectionAsync();
                }
                finally
                {
                    await UpdateView();
                }
            });

            loggerService.EndMethod();
        }

        private async Task StartExposureNotificationAsync()
        {
            loggerService.StartMethod();

            try
            {
                var isSuccess = await exposureNotificationApiService.StartExposureNotificationAsync();
                if (isSuccess)
                {
                    await UpdateView();
                }

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
            try
            {
                UserDialogs.Instance.ShowLoading(AppResources.Loading);
                loggerService.StartMethod();

                var exposureRiskCalculationConfiguration = await exposureRiskCalculationConfigurationRepository
                    .GetExposureRiskCalculationConfigurationAsync(preferCache: true);
                loggerService.Info(exposureRiskCalculationConfiguration.ToString());

                var dailySummaryList = await _exposureDataRepository.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay);
                var dailySummaryMap = dailySummaryList.ToDictionary(ds => ds.GetDateTime());
                var exposureWindowList = await _exposureDataRepository.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay);

                var userExposureInformationList = _exposureDataRepository.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);

                var hasExposure = dailySummaryList.Count() > 0 || userExposureInformationList.Count() > 0;
                var hasHighRiskExposure = userExposureInformationList.Count() > 0;

                foreach (var ew in exposureWindowList.GroupBy(exposureWindow => exposureWindow.GetDateTime()))
                {
                    if (!dailySummaryMap.ContainsKey(ew.Key))
                    {
                        loggerService.Warning($"ExposureWindow: {ew.Key} found, but that is not contained the list of dailySummary.");
                        continue;
                    }

                    var dailySummary = dailySummaryMap[ew.Key];
                    RiskLevel riskLevel = _exposureRiskCalculationService.CalcRiskLevel(
                        dailySummary,
                        ew.ToList(),
                        exposureRiskCalculationConfiguration
                        );
                    if (riskLevel >= RiskLevel.High)
                    {
                        hasHighRiskExposure = true;
                        break;
                    }
                }

                await localNotificationService.DismissExposureNotificationAsync();

                UserDialogs.Instance.HideLoading();

                if (hasHighRiskExposure)
                {
                    await NavigationService.NavigateAsync(nameof(ContactedNotifyPage));
                    return;
                }
                else
                {
                    INavigationParameters navigaitonParameters
                        = ExposureCheckPage.BuildNavigationParams(exposureRiskCalculationConfiguration);
                    await NavigationService.NavigateAsync(nameof(ExposureCheckPage), navigaitonParameters);
                    return;
                }
            }
            catch (Exception exception)
            {
                UserDialogs.Instance.HideLoading();
                loggerService.Exception("Failed to Initialize", exception);
                await dialogService.ShowHomePageUnknownErrorWaringAsync();
            }
            finally
            {
                loggerService.EndMethod();
            }

        });

        public Command OnClickShareApp => new Command(() =>
        {
            loggerService.StartMethod();

            AppUtils.PopUpShare();

            loggerService.EndMethod();
        });

        public Command OnClickLocalNotificationOffWarning => new Command(async () =>
        {
            loggerService.StartMethod();

            var result = await dialogService.ShowLocalNotificationOffWarningAsync();
            if (result)
            {
                externalNavigationService.NavigateAppSettings();
            }

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
            else if (
            statusCodes.Contains(ExposureNotificationStatus.Code_Android.INACTIVATED)
            || statusCodes.Contains(ExposureNotificationStatus.Code_Android.FOCUS_LOST)
            )
            {
                bool isOK = await dialogService.ShowExposureNotificationOffWarningAsync();
                if (isOK)
                {
                    try
                    {
                        await exposureNotificationApiService.StartExposureNotificationAsync();
                        _ = exposureDetectionBackgroundService.ExposureDetectionAsync();
                    }
                    catch (Exception ex)
                    {
                        loggerService.Exception("Failed to fetch exposure key.", ex);
                    }
                    finally
                    {
                        await UpdateView();
                    }
                }
            }
            else if (
            statusCodes.Contains(ExposureNotificationStatus.Code_iOS.Disabled)
            || statusCodes.Contains(ExposureNotificationStatus.Code_iOS.Unauthorized)
            )
            {
                _ = await NavigationService.NavigateAsync(nameof(HowToEnableExposureNotificationsPage));
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
                loggerService.Info("isStopped");
                IsVisibleENStatusActiveLayout = false;
                IsVisibleENStatusUnconfirmedLayout = false;
                IsVisibleENStatusStoppedLayout = true;
            }
            else if (!canConfirmExposure)
            {
                loggerService.Info("canConfirmExposure is false");
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

            IsVisibleLocalNotificationOffWarningLayout = await localNotificationService.IsWarnedLocalNotificationOffAsync();

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

            _ = Task.Run(async () => {
                try
                {
                    await exposureDetectionBackgroundService.ExposureDetectionAsync();
                }
                finally
                {
                    await UpdateView();
                }
            });

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
