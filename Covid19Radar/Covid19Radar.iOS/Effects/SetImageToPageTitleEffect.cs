using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ResolutionGroupName("Covid19Radar")]
[assembly:ExportEffect(typeof(Covid19Radar.iOS.Effects.SetImageToPageTitleEffect), nameof(Covid19Radar.iOS.Effects.SetImageToPageTitleEffect))]
namespace Covid19Radar.iOS.Effects
{
    public class SetImageToPageTitleEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
        }

        protected override void OnDetached()
        {
        }
    }
}