// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Android.Content;
using Android.Views.Accessibility;
using Covid19Radar.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Covid19Radar.Controls.AccessibilityContentView), typeof(AccessibilityContentViewRenderer))]
namespace Covid19Radar.Droid.Renderers
{
    public class AccessibilityContentViewRenderer : ViewRenderer
    {
        private AccessibilityContentView _accessibilityContentView;

        public AccessibilityContentViewRenderer(Context context) : base(context)
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
                    _accessibilityContentView = new AccessibilityContentView(Context);
                    SetNativeControl(_accessibilityContentView);
                }
                // Configure the control and subscribe to event handlers
                _accessibilityContentView.AccessibilityFocused += AccessibilityContentView_AccessibilityFocused;
            }
        }

        private void AccessibilityContentView_AccessibilityFocused(object sender, EventArgs e)
        {
            if (Element is Controls.AccessibilityContentView accessibilityContentView)
            {
                accessibilityContentView.InvokeAccessibilityFocused();
            }
        }
    }

    public class AccessibilityContentView : Android.Views.View
    {
        public event EventHandler<EventArgs> AccessibilityFocused;

        public AccessibilityContentView(Context context) : base(context)
        {
        }

        public override void SendAccessibilityEvent(EventTypes eventType)
        {
            base.SendAccessibilityEvent(eventType);

            AccessibilityManager manager = (AccessibilityManager)Context.GetSystemService(Context.AccessibilityService);
            if (!manager.IsEnabled)
            {
                return;
            }

            switch (eventType)
            {
                case EventTypes.ViewAccessibilityFocused:
                    manager.Interrupt();
                    AccessibilityFocused?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
        }
    }
}

