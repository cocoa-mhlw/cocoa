using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Forms;
using System;
using Acr.UserDialogs;
using Covid19Radar.Views;
using System.Text.RegularExpressions;
using Covid19Radar.Common;
using Covid19Radar.Resources;
using System.Threading.Tasks;
using System.IO;

namespace Covid19Radar.ViewModels
{
    public class NotifyOtherPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IExposureNotificationService exposureNotificationService;
        private readonly ICloseApplication closeApplication;
        private string _diagnosisUid;
        public string DiagnosisUid
        {
            get { return _diagnosisUid; }
            set
            {
                SetProperty(ref _diagnosisUid, value);
                IsEnabled = CheckRegisterButtonEnable();
            }
        }
        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }
        private bool _isVisibleWithSymptomsLayout;
        public bool IsVisibleWithSymptomsLayout
        {
            get { return _isVisibleWithSymptomsLayout; }
            set
            {
                SetProperty(ref _isVisibleWithSymptomsLayout, value);
                IsEnabled = CheckRegisterButtonEnable();
            }
        }
        private bool _isVisibleNoSymptomsLayout;
        public bool IsVisibleNoSymptomsLayout
        {
            get { return _isVisibleNoSymptomsLayout; }
            set
            {
                SetProperty(ref _isVisibleNoSymptomsLayout, value);
                IsEnabled = CheckRegisterButtonEnable();
            }
        }
        private DateTime _diagnosisDate;
        public DateTime DiagnosisDate
        {
            get { return _diagnosisDate; }
            set { SetProperty(ref _diagnosisDate, value); }
        }
        private int errorCount { get; set; }

        public NotifyOtherPageViewModel(INavigationService navigationService, ILoggerService loggerService, IExposureNotificationService exposureNotificationService, ICloseApplication closeApplication) : base(navigationService)
        {
            Title = AppResources.TitileUserStatusSettings;
            this.loggerService = loggerService;
            this.exposureNotificationService = exposureNotificationService;
            this.closeApplication = closeApplication;
            errorCount = 0;
            DiagnosisUid = "";
            DiagnosisDate = DateTime.Today;
        }

        public Command OnClickRegister => (new Command(async () =>
        {
            loggerService.StartMethod();

            var result = await UserDialogs.Instance.ConfirmAsync(AppResources.NotifyOtherPageDiag1Message, AppResources.NotifyOtherPageDiag1Title, AppResources.ButtonAgree, AppResources.ButtonCancel);
            if (!result)
            {
                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDiag2Message,
                    "",
                    AppResources.ButtonOk
                    );

                loggerService.Info($"Canceled by user.");
                loggerService.EndMethod();
                return;
            }

            UserDialogs.Instance.ShowLoading(AppResources.LoadingTextRegistering);

            // Check helthcare authority positive api check here!!
            if (errorCount >= AppConstants.MaxErrorCount)
            {
                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDiagAppClose,
                    AppResources.NotifyOtherPageDiagErrorTitle,
                    AppResources.ButtonOk
                );
                UserDialogs.Instance.HideLoading();
                closeApplication.closeApplication();

                loggerService.Error($"Exceeded the number of trials.");
                loggerService.EndMethod();
                return;
            }

            loggerService.Info($"Number of attempts to submit diagnostic number. ({errorCount + 1} of {AppConstants.MaxErrorCount})");

            if (errorCount > 0)
            {
                var current = errorCount + 1;
                var max = AppConstants.MaxErrorCount;
                await UserDialogs.Instance.AlertAsync(AppResources.NotifyOtherPageDiag3Message,
                    AppResources.NotifyOtherPageDiag3Title + $"{current}/{max}",
                    AppResources.ButtonOk
                    );
                await Task.Delay(errorCount * 5000);
            }


            // Init Dialog
            if (string.IsNullOrEmpty(_diagnosisUid))
            {
                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDiag4Message,
                    AppResources.NotifyOtherPageDiagErrorTitle,
                    AppResources.ButtonOk
                );
                errorCount++;
                UserDialogs.Instance.HideLoading();

                loggerService.Error($"No diagnostic number entered.");
                loggerService.EndMethod();
                return;
            }

            Regex regex = new Regex(AppConstants.positiveRegex);
            if (!regex.IsMatch(_diagnosisUid))
            {
                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDiag5Message,
                    AppResources.NotifyOtherPageDiagErrorTitle,
                    AppResources.ButtonOk
                );
                errorCount++;
                UserDialogs.Instance.HideLoading();

                loggerService.Error($"Incorrect diagnostic number format.");
                loggerService.EndMethod();
                return;
            }

            // Submit the UID
            try
            {
                // EN Enabled Check
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();

                if (!enabled)
                {
                    await UserDialogs.Instance.AlertAsync(
                       AppResources.NotifyOtherPageDiag6Message,
                       AppResources.NotifyOtherPageDiag6Title,
                       AppResources.ButtonOk
                    );
                    UserDialogs.Instance.HideLoading();
                    await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));

                    loggerService.Warning($"Exposure notification is disable.");
                    loggerService.EndMethod();
                    return;
                }

                loggerService.Info($"Submit the processing number.");

                // Submit our diagnosis
                exposureNotificationService.PositiveDiagnosis = _diagnosisUid;
                exposureNotificationService.DiagnosisDate = DiagnosisDate;
                await Xamarin.ExposureNotifications.ExposureNotification.SubmitSelfDiagnosisAsync();
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDialogSubmittedText,
                    AppResources.ButtonComplete,
                    AppResources.ButtonOk
                );
                await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));

                loggerService.Info($"Successfully submit the diagnostic number.");
                loggerService.EndMethod();
            }
            catch (InvalidDataException ex)
            {
                errorCount++;
                UserDialogs.Instance.Alert(
                    AppResources.NotifyOtherPageDialogExceptionTargetDiagKeyNotFound,
                    AppResources.NotifyOtherPageDialogExceptionTargetDiagKeyNotFoundTitle,
                    AppResources.ButtonOk
                );
                loggerService.Exception("Failed to submit UID invalid data.", ex);
                loggerService.EndMethod();
            }
            catch (Exception ex)
            {
                errorCount++;
                UserDialogs.Instance.Alert(
                    AppResources.NotifyOtherPageDialogExceptionText,
                    AppResources.ButtonFailed,
                    AppResources.ButtonOk
                );
                loggerService.Exception("Failed to submit UID.", ex);
                loggerService.EndMethod();
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }));

        public void OnClickRadioButtonIsTrueCommand(string text)
        {
            loggerService.StartMethod();

            if (AppResources.NotifyOtherPageRadioButtonYes.Equals(text))
            {
                IsVisibleWithSymptomsLayout = true;
                IsVisibleNoSymptomsLayout = false;
            }
            else if (AppResources.NotifyOtherPageRadioButtonNo.Equals(text))
            {
                IsVisibleWithSymptomsLayout = false;
                IsVisibleNoSymptomsLayout = true;
            }
            else
            {
                IsVisibleWithSymptomsLayout = false;
                IsVisibleNoSymptomsLayout = false;
            }

            loggerService.Info($"Is visible with symptoms layout: {IsVisibleWithSymptomsLayout}, Is visible no symptoms layout: {IsVisibleNoSymptomsLayout}");
            loggerService.EndMethod();
        }

        public bool CheckRegisterButtonEnable()
        {
            return DiagnosisUid.Length == AppConstants.MaxDiagnosisUidCount && (IsVisibleWithSymptomsLayout || IsVisibleNoSymptomsLayout);
        }
    }
}
