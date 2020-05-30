using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Acr.UserDialogs;
using Covid19Radar.Model;
using System.ComponentModel;

namespace Covid19Radar.ViewModels
{
    public class DebugPageViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private readonly ExposureNotificationService exposureNotificationService;
        private UserDataModel userData;

        public DebugPageViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = "Debug";
            this.userDataService = userDataService;
            this.exposureNotificationService = exposureNotificationService;
            userData = this.userDataService.Get();
            EnStatus = this.exposureNotificationService.CurrentStatusMessage;
            FlgEn = userData.IsExposureNotificationEnabled.ToString();
            FlgLn = userData.IsNotificationEnabled.ToString();
            FlgWl = userData.IsWelcomed.ToString();
            this.userDataService.UserDataChanged += _userDataChanged;
            RaisePropertyChanged(nameof(EnStatus));
            RaisePropertyChanged(nameof(FlgEn));
            RaisePropertyChanged(nameof(FlgLn));
            RaisePropertyChanged(nameof(FlgWl));
        }

        private void _userDataChanged(object sender, UserDataModel e)
        {
            userData = this.userDataService.Get();
            EnStatus = this.exposureNotificationService.CurrentStatusMessage;
            FlgEn = userData.IsExposureNotificationEnabled.ToString();
            FlgLn = userData.IsNotificationEnabled.ToString();
            FlgWl = userData.IsWelcomed.ToString();
            RaisePropertyChanged(nameof(EnStatus));
            RaisePropertyChanged(nameof(FlgEn));
            RaisePropertyChanged(nameof(FlgLn));
            RaisePropertyChanged(nameof(FlgWl));
        }

        public string NativeImplementationName
            => Xamarin.ExposureNotifications.ExposureNotification.OverridesNativeImplementation
                ? "TEST" : "LIVE";

        public string EnStatus { get; set; }
        public string FlgEn { get; set; }
        public string FlgLn { get; set; }
        public string FlgWl { get; set; }

        public string CurrentBatchFileIndex
            => string.Join(", ", userData.ServerBatchNumbers.Select(p => $"{p.Key}={p.Value}"));

        public Command ResetSelfDiagnosis
            => new Command(async () =>
            {
                userData.ClearDiagnosis();
                await userDataService.SetAsync(userData);
                await UserDialogs.Instance.AlertAsync("Self Diagnosis Cleared!");
            });


        public Command ResetExposures
            => new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(() => userData.ExposureInformation.Clear());
                userData.ExposureSummary = null;
                await userDataService.SetAsync(userData);
                await UserDialogs.Instance.AlertAsync("Exposures Cleared!");
            });

        public Command AddExposures
            => new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    userData.ExposureInformation.Add(
                        new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-7), TimeSpan.FromMinutes(30), 70, 6, Xamarin.ExposureNotifications.RiskLevel.High));
                    userData.ExposureInformation.Add(
                        new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-3), TimeSpan.FromMinutes(10), 40, 3, Xamarin.ExposureNotifications.RiskLevel.Low));

                    await userDataService.SetAsync(userData);

                });
            });


        public Command ToggleWelcome => new Command(async () =>
        {
            userData.IsWelcomed = !userData.IsWelcomed;
            await userDataService.SetAsync(userData);
        });

        public Command ToggleEn => new Command(async () =>
        {
            userData.IsExposureNotificationEnabled = !userData.IsExposureNotificationEnabled;
            await userDataService.SetAsync(userData);
        });


        public Command ResetEnabled
            => new Command(async () =>
            {
                using (UserDialogs.Instance.Loading(string.Empty))
                {
                    if (await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
                    {
                        await Xamarin.ExposureNotifications.ExposureNotification.StopAsync();
                    }
                    await userDataService.SetAsync(userData);
                }
                await UserDialogs.Instance.AlertAsync("Last known enabled state reset!");
            });

        public Command ResetBatchFileIndex
            => new Command(async () =>
            {
                userData.ServerBatchNumbers = new Dictionary<string, ulong>(UserDataModel.DefaultServerBatchNumbers);
                await userDataService.SetAsync(userData);
                RaisePropertyChanged(nameof(CurrentBatchFileIndex));
                await UserDialogs.Instance.AlertAsync("Reset Batch file index!");
            });

        public Command ManualTriggerKeyFetch
            => new Command(async () =>
            {
                using (UserDialogs.Instance.Loading("Fetching..."))
                {
                    await Xamarin.ExposureNotifications.ExposureNotification.UpdateKeysFromServer();

                    RaisePropertyChanged(nameof(CurrentBatchFileIndex));
                }
            });
    }
}
