/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Xamarin.Forms;

namespace Covid19Radar.Renderers
{
    public class CustomEntry : Entry
    {
        public event EventHandler DeleteClicked;

        public static readonly BindableProperty BorderColorProperty =
           BindableProperty.Create(nameof(BorderColor),
               typeof(Color),
               typeof(CustomEntry),
               Color.Default);

        /// <summary>
        /// Border Color
        /// </summary>
        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public void TriggerDeleteClicked()
        {
            DeleteClicked?.Invoke(this, null);
        }
    }
}
