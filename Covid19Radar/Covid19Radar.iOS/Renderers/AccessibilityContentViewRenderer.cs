// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Covid19Radar.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Covid19Radar.Controls.AccessibilityContentView), typeof(AccessibilityContentViewRenderer))]
namespace Covid19Radar.iOS.Renderers
{
    public class AccessibilityContentViewRenderer : ViewRenderer
    {
        private AccessibilityContentView _accessibilityContentView;

        public AccessibilityContentViewRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources
                _accessibilityContentView.AccessibilityFocused -= AccessibilityContentView_AccessibilityFocused;
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    _accessibilityContentView = new AccessibilityContentView();
                    SetNativeControl(_accessibilityContentView);
                }
                // Configure the control and subscribe to event handlers
                _accessibilityContentView.AccessibilityFocused += AccessibilityContentView_AccessibilityFocused;

            }
        }

        private void AccessibilityContentView_AccessibilityFocused(object sender, EventArgs e)
        {
            if (Element is Covid19Radar.Controls.AccessibilityContentView accessibilityContentView)
            {
                accessibilityContentView.InvokeAccessibilityFocused();
            }
        }
    }

    public class AccessibilityContentView : UIView
    {
        public event EventHandler<EventArgs> AccessibilityFocused;

        public AccessibilityContentView() : base()
        {
        }

        public override void AccessibilityElementDidBecomeFocused()
        {
            base.AccessibilityElementDidBecomeFocused();
            AccessibilityFocused?.Invoke(this, EventArgs.Empty);
        }
    }
}

