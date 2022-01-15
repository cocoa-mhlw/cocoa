/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.Common
{
    static class AppUtils
    {
        public static async void PopUpShare()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = AppSettings.Instance.AppStoreUrl,
                    Title = Resources.AppResources.AppName
                });
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = AppSettings.Instance.GooglePlayUrl,
                    Title = Resources.AppResources.AppName
                });
            }

        }
    }
}
