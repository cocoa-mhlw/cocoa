// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

[assembly:
    InternalsVisibleTo("Covid19Radar.Android"),
    InternalsVisibleTo("Covid19Radar.iOS")]

namespace Covid19Radar.Controls
{
    public class AccessibilityContentView : ContentView
    {
        public event EventHandler<EventArgs> AccessibilityFocused;

        public AccessibilityContentView() : base()
        {
            AutomationProperties.SetIsInAccessibleTree(this, true);
        }

        internal void InvokeAccessibilityFocused()
        {
            AccessibilityFocused?.Invoke(this, EventArgs.Empty);
        }
    }
}
