using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace Covid19Radar.iOS.Model
{
    public enum EddystoneFrameType
    {
        UnknownFrameType = 0,
        UIDFrameType,
        URLFrameType,
        TelemetryFrameType,
        EIDFrameType
    }
}