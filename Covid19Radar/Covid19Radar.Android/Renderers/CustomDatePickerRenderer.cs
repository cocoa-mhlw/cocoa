/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Android.Content;
using Covid19Radar.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(DatePicker), typeof(CustomDatePickerRendererAndroid))]
namespace Covid19Radar.Droid.Renderers
{
    public class CustomDatePickerRendererAndroid : DatePickerRenderer
    {
        public CustomDatePickerRendererAndroid(Context context) : base(context)
        {
        }
    }
}
