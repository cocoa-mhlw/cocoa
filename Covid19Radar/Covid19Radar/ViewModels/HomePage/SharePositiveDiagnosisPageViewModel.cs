using Acr.UserDialogs;
using Covid19Radar.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SharePositiveDiagnosisPageViewModel : ViewModelBase
    {
        public string DiagnosisUid { get; set; }
        public DateTime? DiagnosisDate { get; set; }

        public SharePositiveDiagnosisPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitileSharePositiveDiagnosis;
        }

        public Command CancelCommand => (new Command(async () =>
        {
            await NavigationService.GoBackAsync();
        }));


        public Command SubmitAndVerifyCommand => (new Command(async () =>
        {
            UserDialogs.Instance.Loading(Resources.AppResources.SharePositiveDiagnosisPageVerifyDialog);

            // Check the diagnosis is valid on the server before asking the native api's for the keys

            if (!await ExposureNotificationHandler.VerifyDiagnosisUid(DiagnosisUid))
            {
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(Resources.AppResources.SharePositiveDiagnosisPageVerifyFailedDialogText, Resources.AppResources.SharePositiveDiagnosisPageVerifyFailedDialogTitle, Resources.AppResources.SharePositiveDiagnosisPageDialogButton);
                return;
            }

            UserDialogs.Instance.Loading(Resources.AppResources.SharePositiveDiagnosisPageSubmittingDialog);

            try
            {
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();

                if (!enabled)
                {
                    await UserDialogs.Instance.AlertAsync(Resources.AppResources.SharePositiveDiagnosisPageNotEnabledENDialogText, Resources.AppResources.SharePositiveDiagnosisPageNotEnabledENDialogTitle, Resources.AppResources.SharePositiveDiagnosisPageDialogButton);
                    return;
                }

                if (string.IsNullOrEmpty(DiagnosisUid))
                {
                    await UserDialogs.Instance.AlertAsync(Resources.AppResources.SharePositiveDiagnosisPageDiagUidIsEmptyText, Resources.AppResources.SharePositiveDiagnosisPageDiagUidIsEmptyTitle, Resources.AppResources.SharePositiveDiagnosisPageDialogButton);
                    return;
                }

                // Set the submitted UID
                LocalStateManager.Instance.LatestDiagnosis.DiagnosisUid = DiagnosisUid;
                LocalStateManager.Instance.LatestDiagnosis.DiagnosisDate = DiagnosisDate ?? DateTime.UtcNow;
                LocalStateManager.Save();

                // Submit our diagnosis
                await Xamarin.ExposureNotifications.ExposureNotification.SubmitSelfDiagnosisAsync();

                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(Resources.AppResources.SharePositiveDiagnosisPageDiagSubmittedText, Resources.AppResources.SharePositiveDiagnosisPageDiagSubmittedTitle, Resources.AppResources.SharePositiveDiagnosisPageDialogButton);

                await NavigationService.GoBackAsync();
            }
            catch
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.Alert(Resources.AppResources.SharePositiveDiagnosisPageDiagExceptionText, Resources.AppResources.SharePositiveDiagnosisPageDiagExceptionTitle, Resources.AppResources.SharePositiveDiagnosisPageDialogButton);
            }
        }));

    }
}
