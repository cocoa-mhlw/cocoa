/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Views;
using Xamarin.Forms;

namespace Covid19Radar
{
    public enum Destination : int
    {
        SplashPage,
        HomePage,
        SettingsPage,
        ContactedNotifyPage,
        NotifyOtherPage,
        SendLogSettingsPage,
    }

    public static class DestinationExtensions
    {
        private static string SplashPagePath = "/" + nameof(SplashPage);
        private static string HomePagePath => "/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage);
        private static string SettingPagePath => "/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(SettingsPage);
        private static string ContactedNotifyPagePath => HomePagePath + "/" + nameof(ContactedNotifyPage);
        private static string NotifyOtherPagePath => HomePagePath + "/" + nameof(NotifyOtherPage);
        private static string SendLogSettingsPage = "/" + nameof(SendLogSettingsPage);

        public static string ToPath(this Destination destination)
        {
            return destination switch
            {
                Destination.SplashPage => SplashPagePath,
                Destination.HomePage => HomePagePath,
                Destination.SettingsPage => SettingPagePath,
                Destination.ContactedNotifyPage => ContactedNotifyPagePath,
                Destination.NotifyOtherPage => NotifyOtherPagePath,
                Destination.SendLogSettingsPage => SendLogSettingsPage,
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
