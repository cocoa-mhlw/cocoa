using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace Covid19Radar.iOS.Model
{
    public class BeaconID : NSObject
    {
        private readonly byte[] _beaconID;
        private readonly BeaconType _beaconType;

        public BeaconType BeaconType { get { return _beaconType; } }

        public BeaconID(BeaconType type, byte[] id)
        {
            _beaconType = type;
            _beaconID = id;
        }

        public override string Description
        {
            get
            {
                if (_beaconType == BeaconType.Eddystone || _beaconType == BeaconType.EddystoneEID)
                {
                    return string.Format("BeaconID beacon: {0}", HexBeaconID());
                }
                else
                    return "BeaconID with invalid type";
            }
        }

        public override string ToString()
        {
            return HexBeaconID();
        }

        private string HexBeaconID()
        {
            return BitConverter.ToString(_beaconID);
        }

        public static bool operator ==(BeaconID a, BeaconID b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a._beaconType == b._beaconType && a._beaconID == b._beaconID;
        }

        public static bool operator !=(BeaconID a, BeaconID b)
        {
            return !(a == b);
        }
    }
}