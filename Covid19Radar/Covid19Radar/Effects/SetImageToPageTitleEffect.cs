using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.Effects
{
    public class SetImageToPageTitleEffect : RoutingEffect
    {
        public SetImageToPageTitleEffect() : base($"Covid19Radar.{nameof(SetImageToPageTitleEffect)}")
        {
        }
    }
}
