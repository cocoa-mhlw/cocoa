
using System;
using System.Collections.Generic;
using System.Linq;

namespace Covid19Radar.Models
{
    public sealed class BeaconDataModel

    {
        public string UUID { get; set; }

        public string Major { get; set; }

        public string Minor { get; set; }

        public double Distance { get; set; }

        public int Rssi { get; set; }
        public int TXPower { get; set; }

        public TimeSpan ElaspedTime { get; set; }
        public DateTime LastDetectTime { get; set; }
    }
}
