/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
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
    public class HomePageViewModel : ViewModelBase
    {
        public string SharingThisAppReadText => $"{AppResources.HomePageDescription5} {AppResources.Button}";

        private readonly ILoggerService loggerService;
        private readonly IUserDataRepository userDataRepository;
        private readonly IExposureNotificationService exposureNotificationService;
        private readonly ILocalNotificationService localNotificationService;
        private readonly IExposureNotificationStatusService exposureNotificationStatusService;
        private readonly IDialogService dialogService;
        private readonly IExternalNavigationService externalNavigationService;
        private readonly IEssentialsService essentialsService;

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
            IExposureNotificationService exposureNotificationService,
            ILocalNotificationService localNotificationService,
            IExposureNotificationStatusService exposureNotificationStatusService,
            IDialogService dialogService,
            IExternalNavigationService externalNavigationService,
            IEssentialsService essentialsService
            ) : base(navigationService)
        {
            Title = AppResources.HomePageTitle;
            this.loggerService = loggerService;
            this.userDataRepository = userDataRepository;
            this.exposureNotificationService = exposureNotificationService;
            this.localNotificationService = localNotificationService;
            this.exposureNotificationStatusService = exposureNotificationStatusService;
            this.dialogService = dialogService;
            this.externalNavigationService = externalNavigationService;
            this.essentialsService = essentialsService;
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

            try
            {
                await localNotificationService.PrepareAsync();

                await exposureNotificationService.StartExposureNotification();
                await exposureNotificationService.FetchExposureKeyAsync();
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed to fetch exposure key.", ex);
            }

            await UpdateView();

            loggerService.EndMethod();
        }

        public Command OnClickExposures => new Command(async () =>
        {
            loggerService.StartMethod();

            await localNotificationService.DismissExposureNotificationAsync();

            var count = exposureNotificationService.GetExposureCountToDisplay();
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

            var enStopReason = exposureNotificationStatusService.ExposureNotificationStoppedReason;

            try
            {
                if (enStopReason == ExposureNotificationStoppedReason.ExposureNotificationOff)
                {
                    bool isOK = await dialogService.ShowExposureNotificationOffWarningAsync();
                    if (isOK)
                    {
                        if (essentialsService.IsAndroid)
                        {
                            try
                            {
                                await exposureNotificationService.StartExposureNotification();
                                await exposureNotificationService.FetchExposureKeyAsync();
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
                        else if (essentialsService.IsIos)
                        {
                            externalNavigationService.NavigateAppSettings();
                        }
                    }
                }
                else if (enStopReason == ExposureNotificationStoppedReason.BluetoothOff)
                {
                    bool isOK = await dialogService.ShowBluetoothOffWarningAsync();
                    if (isOK)
                    {
                        externalNavigationService.NavigateBluetoothSettings();
                    }
                }
                else if (enStopReason == ExposureNotificationStoppedReason.GpsOff)
                {
                    bool isOK = await dialogService.ShowLocationOffWarningAsync();
                    if (isOK)
                    {
                        externalNavigationService.NavigateLocationSettings();
                    }
                }
            }
            catch (PlatformNotSupportedException ex)
            {
                loggerService.Exception("Exception", ex);
            }

            loggerService.EndMethod();
        });

        private async Task UpdateView()
        {
            loggerService.StartMethod();

            var daysOfUse = userDataRepository.GetDaysOfUse();

            PastDate = daysOfUse.ToString();

            await exposureNotificationStatusService.UpdateStatuses();

            switch (exposureNotificationStatusService.ExposureNotificationStatus)
            {
                case ExposureNotificationStatus.Unconfirmed:
                    IsVisibleENStatusActiveLayout = false;
                    IsVisibleENStatusUnconfirmedLayout = true;
                    IsVisibleENStatusStoppedLayout = false;
                    break;
                case ExposureNotificationStatus.Stopped:
                    IsVisibleENStatusActiveLayout = false;
                    IsVisibleENStatusUnconfirmedLayout = false;
                    IsVisibleENStatusStoppedLayout = true;
                    break;
                default:
                    IsVisibleENStatusActiveLayout = true;
                    IsVisibleENStatusUnconfirmedLayout = false;
                    IsVisibleENStatusStoppedLayout = false;

                    var latestUtcDate = userDataRepository.GetLastConfirmedDate();
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

                    break;
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
            await UpdateView();
        }
    }
}
