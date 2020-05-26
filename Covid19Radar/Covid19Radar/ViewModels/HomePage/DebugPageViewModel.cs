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

namespace Covid19Radar.ViewModels
{
    public class DebugPageViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public DebugPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = "Debug";
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
            this.userDataService.UserDataChanged += _userDataChanged;

        }
        private void _userDataChanged(object sender, UserDataModel e)
        {
            userData = this.userDataService.Get();
        }

        public string NativeImplementationName
            => Xamarin.ExposureNotifications.ExposureNotification.OverridesNativeImplementation
                ? "TEST" : "LIVE";

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

        public Command ResetWelcome
            => new Command(async () =>
            {
                userData.IsWelcomed = false;
                await userDataService.SetAsync(userData);
                await UserDialogs.Instance.AlertAsync("Welcome state reset!");
            });

        public Command ResetEnabled
            => new Command(async () =>
            {
                using (UserDialogs.Instance.Loading(string.Empty))
                {
                    if (await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
                        await Xamarin.ExposureNotifications.ExposureNotification.StopAsync();

                    userData.IsExposureNotificationEnabled = false;
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
