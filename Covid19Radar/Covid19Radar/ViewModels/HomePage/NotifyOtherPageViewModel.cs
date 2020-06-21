using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;
using System;
using Acr.UserDialogs;
using Covid19Radar.Views;

namespace Covid19Radar.ViewModels
{
    public class NotifyOtherPageViewModel : ViewModelBase
    {
        public bool IsEnabled { get; set; } = true;
        public string DiagnosisUid { get; set; }
        private int errorCount { get; set; }

        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public NotifyOtherPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
            errorCount = 0;
        }

        public Command OnClickRegister => (new Command(async () =>
        {
            // Check helthcare authority positive api check here!!
            using var dialog = UserDialogs.Instance.Loading(Resources.AppResources.LoadingTextSubmittingDiagnosis);
            dialog.Show();
            if (errorCount > AppConstants.MaxErrorCount)
            {
                await UserDialogs.Instance.AlertAsync(
                    errorCount + "回のエラーが発生しました、アプリケーションを終了します",
                    "登録エラー",
                    Resources.AppResources.ButtonOk
                );
                dialog.Hide();
                Xamarin.Forms.DependencyService.Get<ICloseApplication>().closeApplication();
                return;
            }

            // Tarpit Sleep
            Thread.Sleep(errorCount * 5000);

            // Init Dialog
            if (string.IsNullOrEmpty(DiagnosisUid))
            {
                await UserDialogs.Instance.AlertAsync(
                    "処理番号が入力されていません",
                    "登録エラー",
                    Resources.AppResources.ButtonOk
                );
                errorCount++;
                await userDataService.SetAsync(userData);
                dialog.Hide();
                return;
            }

            Regex regex = new Regex(@"\b[0-9]{8}\b");
            if (!regex.IsMatch(DiagnosisUid))
            {
                await UserDialogs.Instance.AlertAsync(
                    "処理番号のフォーマットが一致していません",
                    "登録エラー",
                    Resources.AppResources.ButtonOk
                );
                errorCount++;
                await userDataService.SetAsync(userData);
                dialog.Hide();
                return;
            }

            // Submit the UID
            IsEnabled = false;
            try
            {
                // EN Enabled Check
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();

                if (!enabled)
                {
                    dialog.Hide();
                    await UserDialogs.Instance.AlertAsync(
                        Resources.AppResources.NotifyOtherPageDialogSubmittedText,
                        Resources.AppResources.ButtonComplete,
                        Resources.AppResources.ButtonOk
                    );
                    await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
                    return;
                }

                // Set the submitted UID
                userData.AddDiagnosis(DiagnosisUid, new DateTimeOffset(DateTime.Now));
                await userDataService.SetAsync(userData);

                // Submit our diagnosis
                await Xamarin.ExposureNotifications.ExposureNotification.SubmitSelfDiagnosisAsync();
                dialog.Hide();

                await UserDialogs.Instance.AlertAsync(
                    Resources.AppResources.NotifyOtherPageDialogSubmittedText,
                    Resources.AppResources.ButtonComplete,
                    Resources.AppResources.ButtonOk
                );

                await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                dialog.Hide();
                UserDialogs.Instance.Alert(
                    Resources.AppResources.NotifyOtherPageDialogExceptionText,
                    Resources.AppResources.ButtonFailed,
                    Resources.AppResources.ButtonOk
                );
            }
            finally
            {
                IsEnabled = true;
            }
        }));

        public Command OnClickAfter => (new Command(async () =>
        {
            var check = await UserDialogs.Instance.ConfirmAsync(
                Resources.AppResources.PositiveRegistrationConfirmText,
                Resources.AppResources.PositiveRegistrationText,
                Resources.AppResources.ButtonNotNow,
                Resources.AppResources.ButtonReturnToRegistration
            );
            if (check)
            {
                await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
            }

        }));
    }
}
