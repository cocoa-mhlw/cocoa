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
        private readonly IUserDataService userDataService;
        private readonly ExposureNotificationService exposureNotificationService;

        public DebugPageViewModel(INavigationService navigationService, IUserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, exposureNotificationService)
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
            _ = await exposureNotificationService.UpdateStatusMessageAsync();
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
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-14), TimeSpan.FromMinutes(5), 10, 6, UserRiskLevel.Lowest));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-13), TimeSpan.FromMinutes(10), 20, 6, UserRiskLevel.Low));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-12), TimeSpan.FromMinutes(15), 50, 6, UserRiskLevel.Medium));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-11), TimeSpan.FromMinutes(20), 50, 6, UserRiskLevel.MediumLow));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-10), TimeSpan.FromMinutes(30), 50, 6, UserRiskLevel.MediumHigh));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-9), TimeSpan.FromMinutes(35), 70, 6, UserRiskLevel.High));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-8), TimeSpan.FromMinutes(40), 70, 6, UserRiskLevel.Highest));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-7), TimeSpan.FromMinutes(45), 80, 6, UserRiskLevel.VeryHigh));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-6), TimeSpan.FromMinutes(50), 80, 6, UserRiskLevel.VeryHigh));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-5), TimeSpan.FromMinutes(55), 70, 6, UserRiskLevel.MediumHigh));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-4), TimeSpan.FromMinutes(0), 70, 6, UserRiskLevel.Medium));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-3), TimeSpan.FromMinutes(5), 70, 6, UserRiskLevel.MediumLow));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-2), TimeSpan.FromMinutes(3), 30, 6, UserRiskLevel.Low));
                    _UserData.ExposureInformation.Add(new UserExposureInfo(DateTime.UtcNow.AddDays(-1), TimeSpan.FromMinutes(20), 70, 6, UserRiskLevel.MediumHigh));
                    await userDataService.SetAsync(_UserData);

                });
            });

        public Command UpdateStatus => new Command(async () =>
        {
            _UserData = this.userDataService.Get();
            _ = await exposureNotificationService.UpdateStatusMessageAsync();
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
                _UserData.ServerBatchNumbers = AppSettings.Instance.GetDefaultBatch();
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
