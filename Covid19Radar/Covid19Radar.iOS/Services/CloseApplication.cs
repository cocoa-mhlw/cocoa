using System.Threading;
using Covid19Radar.Services;

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