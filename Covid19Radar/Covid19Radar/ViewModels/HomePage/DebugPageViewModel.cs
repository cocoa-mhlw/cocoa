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

namespace Covid19Radar.ViewModels
{
    public class DebugPageViewModel : ViewModelBase
    {
        public DebugPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Debug";
        }
        public string NativeImplementationName
    => Xamarin.ExposureNotifications.ExposureNotification.OverridesNativeImplementation
        ? "TEST" : "LIVE";

        public string CurrentBatchFileIndex
            => LocalStateManager.Instance.ServerBatchNumber.ToString();

        public Command ResetSelfDiagnosis => new Command(async () =>
        {
            LocalStateManager.Instance.ClearDiagnosis();
            LocalStateManager.Save();
            await UserDialogs.Instance.AlertAsync("Self Diagnosis Cleared!");
        });


        public Command ResetExposures => new Command(async () =>
           {
               await Device.InvokeOnMainThreadAsync(() => LocalStateManager.Instance.ExposureInformation.Clear());
               LocalStateManager.Instance.ExposureSummary = null;
               LocalStateManager.Save();
               await UserDialogs.Instance.AlertAsync("Exposures Cleared!");
           });

        public Command AddExposures => new Command(async () =>
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                LocalStateManager.Instance.ExposureInformation.Add(new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-7), TimeSpan.FromMinutes(30), 70, 6, Xamarin.ExposureNotifications.RiskLevel.High));
                LocalStateManager.Instance.ExposureInformation.Add(new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-3), TimeSpan.FromMinutes(10), 40, 3, Xamarin.ExposureNotifications.RiskLevel.Low));
                LocalStateManager.Save();
            });
        });

        public Command ResetWelcome => new Command(async () =>
        {
            LocalStateManager.Instance.IsWelcomed = false;
            LocalStateManager.Save();
            await UserDialogs.Instance.AlertAsync("Welcome state reset!");
        });

        public Command ResetEnabled => new Command(async () =>
        {
            using (UserDialogs.Instance.Loading(string.Empty))
            {
                if (await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
                {
                    await Xamarin.ExposureNotifications.ExposureNotification.StopAsync();
                }

                LocalStateManager.Instance.LastIsEnabled = false;
                LocalStateManager.Save();
            }
            await UserDialogs.Instance.AlertAsync("Last known enabled state reset!");
        });

        public Command ResetBatchFileIndex => new Command(async () =>
        {
            LocalStateManager.Instance.ServerBatchNumber = 0;
            LocalStateManager.Save();
            OnPropertyChanged(nameof(CurrentBatchFileIndex));
            await UserDialogs.Instance.AlertAsync("Reset Batch file index!");
        });

        public Command ManualTriggerKeyFetch => new Command(async () =>
        {
            using (UserDialogs.Instance.Loading("Fetching..."))
            {
                await Xamarin.ExposureNotifications.ExposureNotification.UpdateKeysFromServer();
                OnPropertyChanged(nameof(CurrentBatchFileIndex));
            }
        });

    }
}
