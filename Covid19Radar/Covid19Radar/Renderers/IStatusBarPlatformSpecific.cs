using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.Renderers
{
    public interface IStatusBarPlatformSpecific
    {
        void SetStatusBarColor(Color color);
    }
}
