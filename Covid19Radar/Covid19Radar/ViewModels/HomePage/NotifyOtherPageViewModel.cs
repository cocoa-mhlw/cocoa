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

namespace Covid19Radar.ViewModels
{
    public class NotifyOtherPageViewModel : ViewModelBase
    {
        public bool IsEnabled { get; set; }
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
            IsEnabled = true;
        }

        public Command OnClickRegister => (new Command(async () =>
        {
            var result = await UserDialogs.Instance.ConfirmAsync("陽性情報を登録しますか", "登録", "はい", "いいえ");
            if (!result)
            {
                await UserDialogs.Instance.AlertAsync(
                    "キャンセルしました",
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
                    $"登録回数上限になりました。アプリケーションを終了します",
                    "登録エラー",
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
                await UserDialogs.Instance.AlertAsync("登録開始までしばらくそのままでお待ちください",
                    $"登録待ち {current}/{max}回目",
                    Resources.AppResources.ButtonOk
                    );
                Thread.Sleep(errorCount * 5000);
            }


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
                UserDialogs.Instance.HideLoading();
                return;
            }

            Regex regex = new Regex(AppConstants.positiveRegex);
            if (!regex.IsMatch(DiagnosisUid))
            {
                await UserDialogs.Instance.AlertAsync(
                    "処理番号のフォーマットが一致していません",
                    "登録エラー",
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
                        "陽性記録の登録を行う為にCOVID-19接触のログ記録を有効にする必要があります、アプリかOSの設定から有効にしてください。",
                        "COVID-19接触のログ記録を有効にしてください",
                        Resources.AppResources.ButtonOk
                    );
                    UserDialogs.Instance.HideLoading();
                    await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
                    return;
                }

                // Set the submitted UID
                userData.AddDiagnosis(DiagnosisUid, new DateTimeOffset(DateTime.Now));
                await userDataService.SetAsync(userData);

                // Submit our diagnosis
                await Xamarin.ExposureNotifications.ExposureNotification.SubmitSelfDiagnosisAsync();
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(
                    Resources.AppResources.NotifyOtherPageDialogSubmittedText,
                    Resources.AppResources.ButtonComplete,
                    Resources.AppResources.ButtonOk
                );
                await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
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
                IsEnabled = true;
            }
        }));
    }
}
