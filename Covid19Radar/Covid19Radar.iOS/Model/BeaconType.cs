using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace Covid19Radar.iOS.Model
{
    public enum BeaconType
    {
        Unknown = 0,
        Eddystone, // 10 bytes namespace + 6 bytes instance = 16 byte ID
        EddystoneEID // 8 byte ID
    }
}