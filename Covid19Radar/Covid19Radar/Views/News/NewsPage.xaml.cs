﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#nullable enable

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    /// <summary>
    ///  最新情報ページを表します。
    /// </summary>
    /// <remarks>
    ///  内部処理は型'<see cref="Covid19Radar.ViewModels.NewsPageViewModel"/>'にあります。
    /// </remarks>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        public NewsPage()
        {
            this.InitializeComponent();
        }
    }
}
