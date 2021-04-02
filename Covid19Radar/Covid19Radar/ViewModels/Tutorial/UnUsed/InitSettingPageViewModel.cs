/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InitSettingPageViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private readonly ExposureNotificationService exposureNotificationService;
        private UserDataModel userData;

        public InitSettingPageViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = Resources.AppResources.TitleDeviceAccess;
            this.userDataService = userDataService;
            this.exposureNotificationService = exposureNotificationService;
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
            userData.IsNotificationEnabled = true;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(SetupCompletedPage));

        });
    }
}
