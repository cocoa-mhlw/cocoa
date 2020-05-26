using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InitSettingPageViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public InitSettingPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitleDeviceAccess;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }

         public Command OnClickNotNow => new Command(async () =>
        {
            userData.IsExposureNotificationEnabled = false;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(SetupCompletedPage));
        });

        public Command OnClickEnable => new Command(async () =>
        {
            var check = await UserDialogs.Instance.ConfirmAsync(
                "周辺の他のスマートフォンとの間でランダムIDを安全に収集したり共有したりするには、Bluetoothを使用する必要があります。\nCOVID-19(新型コロナウイルス感染症)の検査結果が陽性だった人があなたの周囲にいた場合、 [TODO Replace Public Health Authority] から通知を受け取ることができます。\n濃厚接触の可能性がある日付、期間、電波強度がアプリと共有されます。",
                Resources.AppResources.InitSettingPageDialogTextExposureNotificationTitle,
                Resources.AppResources.DialogButtonOk,
                Resources.AppResources.DialogButtonCancel
            );
            if (!check)
            {
                return;
            }

            UserDialogs.Instance.ShowLoading(Resources.AppResources.LoadingTextEnabling);

            userData.IsExposureNotificationEnabled = true;
            await userDataService.SetAsync(userData);

            if (userData.IsExposureNotificationEnabled && userData.IsWelcomed)
            {
                if (!await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
                {
                    await Xamarin.ExposureNotifications.ExposureNotification.StartAsync();
                }
            }
            UserDialogs.Instance.HideLoading();

            await NavigationService.NavigateAsync(nameof(SetupCompletedPage));

        });
    }
}
