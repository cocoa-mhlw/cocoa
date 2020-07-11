using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;
using System;
using Acr.UserDialogs;
using Covid19Radar.Views;
using System.Text.RegularExpressions;
using System.Threading;
using Covid19Radar.Common;
using Covid19Radar.Resources;
using System.Threading.Tasks;
using System.IO;

namespace Covid19Radar.ViewModels
{
    public class NotifyOtherPageViewModel : ViewModelBase
    {
        private string _diagnosisUid;
        public string DiagnosisUid
        {
            get { return _diagnosisUid; }
            set
            {
                SetProperty(ref _diagnosisUid, value);
                IsEnabled = DiagnosisUid.Length == AppConstants.MaxDiagnosisUidCount;   // validate
            }
        }
        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }
        private int errorCount { get; set; }

        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public NotifyOtherPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
            errorCount = 0;
            DiagnosisUid = "";
        }

        public Command OnClickRegister => (new Command(async () =>
        {
            var result = await UserDialogs.Instance.ConfirmAsync(AppResources.NotifyOtherPageDiag1Message, AppResources.NotifyOtherPageDiag1Title, AppResources.ButtonAgree, AppResources.ButtonCancel);
            if (!result)
            {
                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDiag2Message,
                    "",
                    Resources.AppResources.ButtonOk
                    );
                return;
            }

            UserDialogs.Instance.ShowLoading(Resources.AppResources.LoadingTextRegistering);

            // Check helthcare authority positive api check here!!
            if (errorCount >= AppConstants.MaxErrorCount)
            {
                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDiagAppClose,
                    AppResources.NotifyOtherPageDiagErrorTitle,
                    Resources.AppResources.ButtonOk
                );
                UserDialogs.Instance.HideLoading();
                Xamarin.Forms.DependencyService.Get<ICloseApplication>().closeApplication();
                return;
            }

            if (errorCount > 0)
            {
                var current = errorCount + 1;
                var max = AppConstants.MaxErrorCount;
                await UserDialogs.Instance.AlertAsync(AppResources.NotifyOtherPageDiag3Message,
                    AppResources.NotifyOtherPageDiag3Title + $"{current}/{max}",
                    Resources.AppResources.ButtonOk
                    );
                await Task.Delay(errorCount * 5000);
            }


            // Init Dialog
            if (string.IsNullOrEmpty(_diagnosisUid))
            {
                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDiag4Message,
                    AppResources.NotifyOtherPageDiagErrorTitle,
                    Resources.AppResources.ButtonOk
                );
                errorCount++;
                await userDataService.SetAsync(userData);
                UserDialogs.Instance.HideLoading();
                return;
            }

            Regex regex = new Regex(AppConstants.positiveRegex);
            if (!regex.IsMatch(_diagnosisUid))
            {
                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDiag5Message,
                    AppResources.NotifyOtherPageDiagErrorTitle,
                    Resources.AppResources.ButtonOk
                );
                errorCount++;
                await userDataService.SetAsync(userData);
                UserDialogs.Instance.HideLoading();
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
                       Resources.AppResources.ButtonOk
                    );
                    UserDialogs.Instance.HideLoading();
                    await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
                    return;
                }

                // Set the submitted UID
                userData.AddDiagnosis(_diagnosisUid, new DateTimeOffset(DateTime.Now));
                await userDataService.SetAsync(userData);

                // Submit our diagnosis
                await Xamarin.ExposureNotifications.ExposureNotification.SubmitSelfDiagnosisAsync();
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(
                    Resources.AppResources.NotifyOtherPageDialogSubmittedText,
                    Resources.AppResources.ButtonComplete,
                    Resources.AppResources.ButtonOk
                );
                await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
            }
            catch (InvalidDataException ex)
            {
                errorCount++;
                UserDialogs.Instance.Alert(
                    Resources.AppResources.NotifyOtherPageDialogExceptionTargetDiagKeyNotFound,
                    Resources.AppResources.NotifyOtherPageDialogExceptionTargetDiagKeyNotFoundTitle,
                    Resources.AppResources.ButtonOk
                );
            }
            catch (Exception ex)
            {
                errorCount++;
                UserDialogs.Instance.Alert(
                    Resources.AppResources.NotifyOtherPageDialogExceptionText,
                    Resources.AppResources.ButtonFailed,
                    Resources.AppResources.ButtonOk
                );
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }));
    }
}
