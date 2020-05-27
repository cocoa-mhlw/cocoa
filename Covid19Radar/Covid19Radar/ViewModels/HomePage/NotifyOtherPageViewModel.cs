using System.Collections.Generic;
using System.Linq;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;
using System;
using System.Windows.Input;
using Prism.Navigation.Xaml;
using Acr.UserDialogs;
using Covid19Radar.Renderers;
using Covid19Radar.Views;
using Xamarin.Essentials;

namespace Covid19Radar.ViewModels
{
    public class NotifyOtherPageViewModel : ViewModelBase
    {
        public bool IsEnabled { get; set; } = true;
        public string DiagnosisUid { get; set; }
        public DateTime? DiagnosisTimestamp { get; set; } = DateTime.Now;

        public DateTime? MaxDate { get; } = DateTime.Today.AddMonths(1);
        public DateTime? MinDate { get; } = DateTime.Today.AddMonths(-1);

        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public NotifyOtherPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }


        public Command OnClickRegister => (new Command(async () =>
        {
            // Verify  Check the parameters

            if (string.IsNullOrEmpty(DiagnosisUid))
            {
                // Check gov's positive api check here!!

                await UserDialogs.Instance.AlertAsync("Please provide a valid Diagnosis ID", "Invalid Diagnosis ID", "OK");
                return;
            }
            if (!DiagnosisTimestamp.HasValue || DiagnosisTimestamp.Value > DateTime.Now)
            {
                await UserDialogs.Instance.AlertAsync("Please provide a valid Test Date", "Invalid Test Date", "OK");
                return;
            }

            // Submit the UID
            using var dialog = UserDialogs.Instance.Loading("Submitting Diagnosis...");
            IsEnabled = false;
            try
            {
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();

                if (!enabled)
                {
                    dialog.Hide();

                    await UserDialogs.Instance.AlertAsync("Please enable Exposure Notifications before submitting a diagnosis.", "Exposure Notifications Disabled", "OK");
                    return;
                }

                // Set the submitted UID
                userData.AddDiagnosis(DiagnosisUid, new DateTimeOffset(DiagnosisTimestamp.Value));
                await userDataService.SetAsync(userData);

                // Submit our diagnosis
                await Xamarin.ExposureNotifications.ExposureNotification.SubmitSelfDiagnosisAsync();

                dialog.Hide();

                await UserDialogs.Instance.AlertAsync("Diagnosis Submitted", "Complete", "OK");
                await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                dialog.Hide();
                UserDialogs.Instance.Alert("Please try again later.", "Failed", "OK");
            }
            finally
            {
                IsEnabled = true;
            }


        }));

        public Command OnClickAfter => (new Command(async () =>
        {
            var check = await UserDialogs.Instance.ConfirmAsync("あとで設定しますか?", "陽性登録", "後にする", "登録へ戻る");
            if (check)
            {
                await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
            }

        }));
    }
}
