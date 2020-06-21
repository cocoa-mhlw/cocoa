using Covid19Radar.Services;
using Prism.Navigation;
using System;
using System.Linq;
using Xamarin.Forms;
using Acr.UserDialogs;
using Covid19Radar.Model;

namespace Covid19Radar.ViewModels
{
    public class DebugPageViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private readonly ExposureNotificationService exposureNotificationService;

        public DebugPageViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = "Debug";
            this.userDataService = userDataService;
            this.exposureNotificationService = exposureNotificationService;
            _UserData = this.userDataService.Get();
            _EnMessage = this.exposureNotificationService.CurrentStatusMessage;
            this.userDataService.UserDataChanged += _userDataChanged;
        }

        private async void _userDataChanged(object sender, UserDataModel e)
        {
            _UserData = this.userDataService.Get();
            _ = await exposureNotificationService.UpdateStatusMessage();
            _EnMessage = this.exposureNotificationService.CurrentStatusMessage;
        }

        public string NativeImplementationName
            => Xamarin.ExposureNotifications.ExposureNotification.OverridesNativeImplementation
                ? "TEST" : "LIVE";

        private UserDataModel _UserData;

        public UserDataModel UserData
        {
            get { return _UserData; }
            set { SetProperty(ref _UserData, value); }
        }

        private string _EnMessage;

        public string EnMessage
        {
            get { return _EnMessage; }
            set { SetProperty(ref _EnMessage, value); }
        }

        public string CurrentBatchFileIndex
            => string.Join(", ", _UserData.ServerBatchNumbers.Select(p => $"{p.Key}={p.Value}"));

        public Command ResetSelfDiagnosis
            => new Command(async () =>
            {
                _UserData.ClearDiagnosis();
                await userDataService.SetAsync(_UserData);
                await UserDialogs.Instance.AlertAsync("Self Diagnosis Cleared!");
            });


        public Command ResetExposures
            => new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(() => _UserData.ExposureInformation.Clear());
                _UserData.ExposureSummary = null;
                await userDataService.SetAsync(_UserData);
                await UserDialogs.Instance.AlertAsync("Exposures Cleared!");
            });

        public Command AddExposures
            => new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    _UserData.ExposureInformation.Add(
                        new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-7), TimeSpan.FromMinutes(30), 70, 6, Xamarin.ExposureNotifications.RiskLevel.High));
                    _UserData.ExposureInformation.Add(
                        new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-3), TimeSpan.FromMinutes(10), 40, 3, Xamarin.ExposureNotifications.RiskLevel.Low));

                    await userDataService.SetAsync(_UserData);

                });
            });

        public Command UpdateStatus => new Command(async () =>
        {
            _UserData = this.userDataService.Get();
            _ = await exposureNotificationService.UpdateStatusMessage();
            _EnMessage = this.exposureNotificationService.CurrentStatusMessage;
        });


        public Command ToggleWelcome => new Command(async () =>
        {
            _UserData.IsOptined = !_UserData.IsOptined;
            await userDataService.SetAsync(_UserData);
        });

        public Command ToggleEn => new Command(async () =>
        {
            _UserData.IsExposureNotificationEnabled = !_UserData.IsExposureNotificationEnabled;
            await userDataService.SetAsync(_UserData);
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
                    await userDataService.SetAsync(_UserData);
                }
                await UserDialogs.Instance.AlertAsync("Last known enabled state reset!");
            });

        public Command ResetBatchFileIndex
            => new Command(async () =>
            {
                _UserData.ServerBatchNumbers = AppSettings.Instance.GetDefaultDefaultBatch();
                await userDataService.SetAsync(_UserData);
                RaisePropertyChanged(nameof(CurrentBatchFileIndex));
                await UserDialogs.Instance.AlertAsync("Reset Batch file index!");
            });

        public Command ManualTriggerKeyFetch
            => new Command(async () =>
            {
                using (UserDialogs.Instance.Loading("Fetching..."))
                {
                    await exposureNotificationService.FetchExposureKeyAsync();

                    RaisePropertyChanged(nameof(CurrentBatchFileIndex));
                }
            });
    }
}
