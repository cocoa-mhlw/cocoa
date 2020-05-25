using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Covid19Radar.iOS.Services;
using Covid19Radar.Services;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApplication))]
namespace Covid19Radar.iOS.Services
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            Thread.CurrentThread.Abort();
        }
    }
}