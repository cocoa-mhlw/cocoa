/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DebugPageViewModel : ViewModelBase
    {
        private readonly ITermsUpdateService _termsUpdateService;
        private readonly IExposureConfigurationRepository _exposureConfigurationRepository;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly AbsExposureNotificationApiService _exposureNotificationApiService;
        private readonly AbsExposureDetectionBackgroundService _exposureDetectionBackgroundService;
        private readonly ICloseApplicationService _closeApplicationService;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly ILocalNotificationService _localNotificationService;

        private string _debugInfo;
        public string DebugInfo
        {
            get { return _debugInfo; }
            set { SetProperty(ref _debugInfo, value); }
        }

        private string _serverConfigurationInfo;
        public string ServerConfigurationInfo
        {
            get { return _serverConfigurationInfo; }
            set { SetProperty(ref _serverConfigurationInfo, value); }
        }

        public async override void OnAppearing()
        {
            base.OnAppearing();

            await UpdateInfo();
        }

        public async Task UpdateInfo(string exception = "")
        {
            string os = Device.RuntimePlatform;
#if DEBUG
            os += ",DEBUG";
#endif
#if USE_MOCK
            os += ",USE_MOCK";
#endif

            // debug info for ./SplashPageViewModel.cs
            var termsUpdateInfo = await _termsUpdateService.GetTermsUpdateInfo() ?? new Model.TermsUpdateInfoModel();

            var termsOfServiceUpdateDateTimeUtc = "Not Available";
            if (termsUpdateInfo.TermsOfService != null)
            {
                termsOfServiceUpdateDateTimeUtc = termsUpdateInfo.TermsOfService.UpdateDateTimeUtc.ToString();
            }

            var privacyPolicyUpdateDateTimeUtc = "Not Available";
            if (termsUpdateInfo.PrivacyPolicy != null)
            {
                privacyPolicyUpdateDateTimeUtc = termsUpdateInfo.PrivacyPolicy.UpdateDateTimeUtc.ToString();
            }

            var lastProcessTekTimestampList = new List<LastProcessTekTimestamp>();

            foreach(var region in AppSettings.Instance.SupportedRegions)
            {
                var ticks = await _userDataRepository.GetLastProcessDiagnosisKeyTimestampAsync(region);
                var lastProcessTekTimestamp = new LastProcessTekTimestamp()
                {
                    Region = region,
                    Ticks = ticks
                };
                lastProcessTekTimestampList.Add(lastProcessTekTimestamp);
            };

            string regionString = string.Join(",", AppSettings.Instance.SupportedRegions);
            string lastProcessTekTimestampsStr = string.Join("\n  ", lastProcessTekTimestampList.Select(lptt => lptt.ToString()));

            var exposureNotificationStatus = await _exposureNotificationApiService.IsEnabledAsync();

            var dailySummaryCount = (await _exposureDataRepository.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay)).Count();
            var exposureWindowCount = (await _exposureDataRepository.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay)).Count();

            // ../../settings.json
            var settings = new[] {
                $"Build: {os}",
                $"Version: {AppSettings.Instance.AppVersion}",
                $"Region: {regionString}",
                $"CdnUrl: {AppSettings.Instance.CdnUrlBase}",
                $"ApiUrl: {AppSettings.Instance.ApiUrlBase}",
                $"TermsOfServiceUpdatedDateTimeUtc: {termsOfServiceUpdateDateTimeUtc}",
                $"PrivacyPolicyUpdatedDateTimeUtc: {privacyPolicyUpdateDateTimeUtc}",
                $"StartDate: {_userDataRepository.GetStartDate().ToLocalTime().ToString("F")}",
                $"DaysOfUse: {_userDataRepository.GetDaysOfUse()}",
                $"Legacy-V1 ExposureCount: {_exposureDataRepository.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay).Count()}",
                $"DailySummaryCount: {dailySummaryCount}",
                $"ExposureWindowCount: {exposureWindowCount}",
                $"LastProcessTekTimestamp: {lastProcessTekTimestampsStr}",
                $"ENstatus: {exposureNotificationStatus}",
                $"Now: {DateTime.Now.ToLocalTime().ToString("F")}",
                exception
            };
            DebugInfo = string.Join(Environment.NewLine, settings);

            // ServerConfiguration
            var serverRegions = string.Join(",", _serverConfigurationRepository.Regions);
            var diagnosisKeyListProvideServerUrls = _serverConfigurationRepository.DiagnosisKeyListProvideServerUrls
                .Select(url => $"    * {url}");

            var serverConfiguration = new[]
            {
                $"Regions: {serverRegions}",
                $"DiagnosisKeyRegisterApiEndpoint: {_serverConfigurationRepository.DiagnosisKeyRegisterApiEndpoint}",
                $"DiagnosisKeyListProvideServerEndpoint: {_serverConfigurationRepository.DiagnosisKeyListProvideServerEndpoint}",
                $"ExposureConfigurationUrl: {_serverConfigurationRepository.ExposureConfigurationUrl}",
                $"ExposureRiskCalculationConfigurationUrl: {_serverConfigurationRepository.ExposureRiskCalculationConfigurationUrl}",
                $"ExposureDataCollectServerEndpoint: {_serverConfigurationRepository.ExposureDataCollectServerEndpoint}",
                $"EventLogApiEndpoint: {_serverConfigurationRepository.EventLogApiEndpoint}",
                $"UserRegisterApiEndpoint: {_serverConfigurationRepository.UserRegisterApiEndpoint}",
                $"InquiryLogApiUrl: {_serverConfigurationRepository.InquiryLogApiUrl}",
                $"LogStorageEndpoint: {_serverConfigurationRepository.LogStorageEndpoint}",
            };
            ServerConfigurationInfo = string.Join(Environment.NewLine, serverConfiguration);
        }

        public DebugPageViewModel(
            INavigationService navigationService,
            ITermsUpdateService termsUpdateService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            IUserDataRepository userDataRepository,
            IExposureDataRepository exposureDataRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            AbsExposureDetectionBackgroundService exposureDetectionBackgroundService,
            ICloseApplicationService closeApplicationService,
            IServerConfigurationRepository serverConfigurationRepository,
            ILocalNotificationService localNotificationService
            ) : base(navigationService)
        {
            Title = "Title:Debug";
            _termsUpdateService = termsUpdateService;
            _exposureConfigurationRepository = exposureConfigurationRepository;
            _userDataRepository = userDataRepository;
            _exposureDataRepository = exposureDataRepository;
            _exposureNotificationApiService = exposureNotificationApiService;
            _exposureDetectionBackgroundService = exposureDetectionBackgroundService;
            _closeApplicationService = closeApplicationService;
            _serverConfigurationRepository = serverConfigurationRepository;
            _localNotificationService = localNotificationService;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            await _serverConfigurationRepository.LoadAsync();

            _ = UpdateInfo("Initialize");
        }

        public Command OnClickReload => new Command(async () => await UpdateInfo("Reload"));

        public Command OnClickStartExposureNotification => new Command(async () =>
        {
            UserDialogs.Instance.ShowLoading("Starting ExposureNotification...");
            await _exposureNotificationApiService.StartAsync();
            UserDialogs.Instance.HideLoading();
            await UpdateInfo("StartExposureNotification");
        });

        public Command OnClickFetchExposureKeyAsync => new Command(async () =>
        {
            var exception = "FetchExposureKeyAsync";
            try
            {
                await _exposureDetectionBackgroundService.ExposureDetectionAsync();
            }
            catch (Exception ex)
            {
                exception += $":Exception: {ex}";
            }
            await UpdateInfo(exception);
        });

        // see ../Settings/SettingsPageViewModel.cs
        public Command OnClickStopExposureNotification => new Command(async () =>
        {
            UserDialogs.Instance.ShowLoading("Stopping ExposureNotification...");
            await _exposureNotificationApiService.StopAsync();
            UserDialogs.Instance.HideLoading();
            await UpdateInfo("StopExposureNotification");
        });

        public Command OnClickShowExposureNotification => new Command(async () =>
        {
            await _localNotificationService.ShowExposureNotificationAsync();
        });

        public Command OnClickExportExposureWindow => new Command(async () =>
        {
            var exposureWindows = await _exposureDataRepository.GetExposureWindowsAsync();
            var csv = ConvertCsv(exposureWindows);
            var hashString = ConvertSha256(csv);

            var fileName = $"exposure_window-{DeviceInfo.Name}-{hashString}.csv";
            var file = Path.Combine(FileSystem.CacheDirectory, fileName);
            File.WriteAllText(file, csv);

            var shareFile = new ShareFile(file);
            await Share.RequestAsync(new ShareFileRequest
            {
                File = shareFile
            });
        });

        private string ConvertCsv(List<ExposureWindow> exposureWindows)
        {
            var flattenWindowLines = exposureWindows.Select((window, index) =>
            {
                var humanReadableDateMillisSinceEpoch = DateTimeOffset.UnixEpoch
                    .AddMilliseconds(window.DateMillisSinceEpoch).UtcDateTime
                    .ToLocalTime()
                    .ToString("D", CultureInfo.CurrentCulture);
                var connmaSeparatedWindow = $"{index},{window.CalibrationConfidence},{humanReadableDateMillisSinceEpoch},{window.Infectiousness},{window.ReportType}";
                var flattenWindow = window.ScanInstances.Select(scanInstance =>
                {
                    var connmaSeparatedScanInstance = $"{scanInstance.MinAttenuationDb},{scanInstance.SecondsSinceLastScan},{scanInstance.TypicalAttenuationDb}";
                    return $"{connmaSeparatedWindow},{connmaSeparatedScanInstance}";
                });
                return String.Join("\n", flattenWindow);
            });

            var csv = new StringBuilder();
            csv.AppendLine("ExposureWindowIndex,CalibrationConfidence,DateMillisSinceEpoch,Infectiousness,ReportType,MinAttenuationDb,SecondsSinceLastScan,TypicalAttenuationDb");
            csv.AppendLine(String.Join("\n", flattenWindowLines));
            return csv.ToString();
        }

        private string ConvertSha256(string text)
        {
            using var sha = SHA256.Create();
            var textBytes = Encoding.UTF8.GetBytes(text);
            var hash = sha.ComputeHash(textBytes);
            return BitConverter.ToString(hash).Replace("-", string.Empty); 
        }

        public Command OnClickRemoveStartDate => new Command(async () =>
        {
            _userDataRepository.RemoveStartDate();
            await UpdateInfo("RemoveStartDate");
        });

        public Command OnClickRemoveExposureInformation => new Command(async () =>
        {
            _exposureDataRepository.RemoveExposureInformation();
            await UpdateInfo("RemoveExposureInformation");
        });

        public Command OnClickRemoveConfiguration => new Command(async () =>
        {

             await _exposureConfigurationRepository.RemoveExposureConfigurationAsync();
             await UpdateInfo("RemoveConfiguration");
        });

        public Command OnClickRemoveLastProcessTekTimestamp => new Command(async () =>
        {
            await _userDataRepository.RemoveLastProcessDiagnosisKeyTimestampAsync();
            await UpdateInfo("RemoveLastProcessTekTimestamp");
        });

        public Command OnClickRemoveAllUpdateDate => new Command(async () =>
        {
            _userDataRepository.RemoveAllUpdateDate();
            await UpdateInfo("RemoveAllUpdateDate");
        });

        public Command OnClickReAgreeTermsOfServicePage => new Command(async () =>
        {
            var termsUpdateInfoModel = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { Text = "DEBUG  利用規約の変更", UpdateDateTimeJst = new DateTime(2022, 03, 14) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { Text = "DEBUG プライバシーポリシーの変更", UpdateDateTimeJst = new DateTime(2022, 03, 14) }
            };
            var navigationParams = ReAgreeTermsOfServicePage.BuildNavigationParams(termsUpdateInfoModel, Destination.DebugPage);
            _ = await NavigationService.NavigateAsync("/" + nameof(ReAgreeTermsOfServicePage), navigationParams);
        });

        public Command OnClickReAgreePrivacyPolicyPage => new Command(async () =>
        {
            var privacyPolicyUpdated = new TermsUpdateInfoModel.Detail { Text = "DEBUG プライバシーポリシーの変更", UpdateDateTimeJst = new DateTime(2022, 03, 14) };
            var navigationParams = ReAgreePrivacyPolicyPage.BuildNavigationParams(privacyPolicyUpdated, Destination.DebugPage);
            _ = await NavigationService.NavigateAsync("/" + nameof(ReAgreePrivacyPolicyPage), navigationParams);
        });

        public Command OnClickQuit => new Command(() =>
        {
            Application.Current.Quit();
            _closeApplicationService.CloseApplication();
        });

        private class LastProcessTekTimestamp
        {
            internal string Region;

            internal long Ticks;

            internal DateTimeOffset DateTime
                => DateTimeOffset.FromUnixTimeMilliseconds(Ticks).ToLocalTime();

            public override string ToString()
                => $"{Region} - {DateTime:F}({Ticks})";
        }
    }
}
