/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Views;
using Covid19Radar.Views.EndOfService;
using Xamarin.Forms;

namespace Covid19Radar
{
    public enum Destination : int
    {
        SplashPage,
        ContactedNotifyPage,
        NotifyOtherPage,
        EndOfServiceNotice,
        EndOfService,
    }

    public static class DestinationExtensions
    {
        private static string SplashPagePath = "/" + nameof(SplashPage);
        private static string EndOfServiceNoticePath => $"/{nameof(MenuPage)}/{nameof(NavigationPage)}/{nameof(EndOfServiceNoticePage)}";
        private static string EndOfServicePath => $"/{nameof(EndOfServicePage)}";

        public static string ToPath(this Destination destination)
        {
            return destination switch
            {
                Destination.SplashPage => SplashPagePath,
                Destination.EndOfServiceNotice => EndOfServiceNoticePath,
                Destination.EndOfService => EndOfServicePath,
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
