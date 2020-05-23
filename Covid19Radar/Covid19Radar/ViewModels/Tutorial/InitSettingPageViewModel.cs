using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InitSettingPageViewModel : ViewModelBase
    {
        public InitSettingPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitleDeviceAccess;
        }

        public Command OnClickNotNow => new Command(async () =>
        {
            LocalStateManager.Instance.LastIsEnabled = false;
            LocalStateManager.Save();
            await NavigationService.NavigateAsync(nameof(SetupCompletedPage));
        });
        public Command OnClickEnable => new Command(async () =>
        {
            var check = await UserDialogs.Instance.ConfirmAsync("周辺の他のスマートフォンとの間でランダムIDを安全に収集したり共有したりするには、Bluetoothを使用する必要があります。\nCOVID-19(新型コロナウイルス感染症)の検査結果が陽性だった人があなたの周囲にいた場合、 [TODO Replace Public Health Authority] から通知を受け取ることができます。\n濃厚接触の可能性がある日付、期間、電波強度がアプリと共有されます。", "COVID-19(新型コロナウイルス感染症)の濃厚接触の可能性の通知をオンにしますか?", "OK", "Cancel");
            if (!check)
            {
                return;
            }

            UserDialogs.Instance.ShowLoading("有効にしています。");

            LocalStateManager.Instance.LastIsEnabled = true;
            LocalStateManager.Save();

            if (LocalStateManager.Instance.LastIsEnabled && LocalStateManager.Instance.IsWelcomed)
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
