using System.Collections.Generic;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage4ViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public TutorialPage4ViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitleHowItWorks;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }

        public Command OnClickEnable => new Command(async () =>
        {
            /*
            var check = await UserDialogs.Instance.ConfirmAsync(
                "周辺の他のスマートフォンとの間でランダムIDを安全に収集したり共有したりするには、Bluetoothを使用する必要があります。\nCOVID-19(新型コロナウイルス感染症)の検査結果が陽性だった人があなたの周囲にいた場合、 [TODO Replace Public Health Authority] から通知を受け取ることができます。\n濃厚接触の可能性がある日付、期間、電波強度がアプリと共有されます。",
                Resources.AppResources.InitSettingPageDialogExposureNotificationTitle,
                Resources.AppResources.ButtonOk,
                Resources.AppResources.ButtonCancel
            );
            if (!check){

            }
*/
            userData.IsExposureNotificationEnabled = true;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage5));
        });
        public Command OnClickDisable => new Command(async () =>
        {
            userData.IsExposureNotificationEnabled = false;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage5));
        });
    }
}