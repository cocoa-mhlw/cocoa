using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBluetooth;
using Foundation;
using UIKit;

namespace Covid19Radar.iOS.Model
{
    public class BeaconInfo : NSObject
    {
        private const byte EddystoneUIDFrameTypeID = 0x00;
        private const byte EddystoneURLFrameTypeID = 0x10;
        private const byte EddystoneTLMFrameTypeID = 0x20;
        private const byte EddystoneEIDFrameTypeID = 0x30;
        private const byte iBeaconFrameTypeID = 0x02;

        public BeaconID BeaconID { get; private set; }
        public int TxPower { get; private set; }
        public int Rssi { get; private set; }
        public NSData Telemetry { get; private set; }

        public BeaconInfo(BeaconID beaconID, int txPower, int rssi, NSData telemetry)
        {
            BeaconID = beaconID;
            TxPower = txPower;
            Rssi = rssi;
            Telemetry = telemetry;
        }

        public static EddystoneFrameType FrameTypeForFrame(NSDictionary advertisementFrameList)
        {
            var frameData = advertisementFrameList.ObjectForKey(CBUUID.FromString("FEAA")) as NSData;

            if (frameData != null && frameData.Length >= 1)
            {
                var frameBytes = frameData.ToArray();

                if (frameBytes[0] == EddystoneUIDFrameTypeID)
                    return EddystoneFrameType.UIDFrameType;
                else if (frameBytes[0] == EddystoneTLMFrameTypeID)
                    return EddystoneFrameType.TelemetryFrameType;
                else if (frameBytes[0] == EddystoneEIDFrameTypeID)
                    return EddystoneFrameType.EIDFrameType;
                else if (frameBytes[0] == EddystoneURLFrameTypeID)
                    return EddystoneFrameType.URLFrameType;
                else
                    return EddystoneFrameType.UnknownFrameType;
            }

            return EddystoneFrameType.UnknownFrameType;
        }

        public static NSData TelemetryDataForFrame(NSDictionary advertisementFrameList)
        {
            return advertisementFrameList.ObjectForKey(CBUUID.FromString("FEAA")) as NSData;
        }

        public static BeaconInfo BeaconInfoForUIDFrameData(NSDictionary advertisementFrameList, NSData telemetry, int rssi)
        {
            var frameData = advertisementFrameList.ObjectForKey(CBUUID.FromString("FEAA")) as NSData;

            if (frameData.Length > 1)
            {
                var frameBytes = frameData.ToArray();

                if (frameBytes[0] != EddystoneUIDFrameTypeID)
                {
                    System.Diagnostics.Debug.WriteLine("Unexpected non UID Frame passed to BeaconInfoForUIDFrameData.");
                    return null;
                }
                else if (frameBytes.Length < 18)
                    System.Diagnostics.Debug.WriteLine("Frame Data for UID Frame unexpectedly truncated in BeaconInfoForUIDFrameData.");

                var txPower = Convert.ToInt32(frameBytes[1]);

                var beaconID = new byte[frameBytes.Length - 2];
                Array.Copy(frameBytes, 2, beaconID, 0, beaconID.Length);

                var bid = new BeaconID(BeaconType.Eddystone, beaconID);

                return new BeaconInfo(bid, txPower, rssi, telemetry);
            }

            return null;
        }

        public static BeaconInfo BeaconInfoForEIDFrameData(NSDictionary advertisementFrameList, NSData telemetry, int rssi)
        {
            var frameData = advertisementFrameList.ObjectForKey(CBUUID.FromString("FEAA")) as NSData;

            if (frameData.Length > 1)
            {
                var frameBytes = frameData.ToArray();

                if (frameBytes[0] != EddystoneEIDFrameTypeID)
                {
                    System.Diagnostics.Debug.WriteLine("Unexpected non EID Frame passed to BeaconInfoForEIDFrameData.");
                    return null;
                }
                else if (frameBytes.Length < 10)
                    System.Diagnostics.Debug.WriteLine("Frame Data for EID Frame unexpectedly truncated in BeaconInfoForEIDFrameData.");

                var txPower = Convert.ToInt32(frameBytes[1]);

                var beaconID = new byte[frameBytes.Length - 2];
                Array.Copy(frameBytes, 2, beaconID, 0, beaconID.Length);

                var bid = new BeaconID(BeaconType.Eddystone, beaconID);

                return new BeaconInfo(bid, txPower, rssi, telemetry);
            }

            return null;
        }

        public static string ParseUrlFromFrame(NSDictionary advertisementFrameList)
        {
            var frameData = advertisementFrameList.ObjectForKey(CBUUID.FromString("FEAA")) as NSData;

            if (frameData.Length > 0)
            {
                var count = frameData.Length;
                var frameBytes = frameData.ToArray();

                var urlPrefix = URLPrefixFromByte(frameBytes[2]);

                var output = urlPrefix;
                for (nuint i = 3; i < count; i++)
                    output += EncodedStringFromByte(frameBytes[i]);

                return output;
            }

            return null;
        }

        public override string Description
        {
            get
            {
                return this.ToString();
            }
        }

        public override string ToString()
        {
            return BeaconID.BeaconType == BeaconType.Eddystone ?
                $"Eddystone {BeaconID}, txPower: {TxPower}, RSSI: {Rssi}"
                :
                $"Eddystone EID {BeaconID}, txPower: {TxPower}, RSSI: {Rssi}";
        }

        public static string URLPrefixFromByte(byte schemeId)
        {
            switch (schemeId)
            {
                case 0x00:
                    return "http://www.";
                case 0x01:
                    return "https://www.";
                case 0x02:
                    return "http://";
                case 0x03:
                    return "https://";
                default:
                    return string.Empty;
            }
        }

        public static string EncodedStringFromByte(byte charVal)
        {
            switch (charVal)
            {
                case 0x00:
                    return ".com/";
                case 0x01:
                    return ".org/";
                case 0x02:
                    return ".edu/";
                case 0x03:
                    return ".net/";
                case 0x04:
                    return ".info/";
                case 0x05:
                    return ".biz/";
                case 0x06:
                    return ".gov/";
                case 0x07:
                    return ".com";
                case 0x08:
                    return ".org";
                case 0x09:
                    return ".edu";
                case 0x0a:
                    return ".net";
                case 0x0b:
                    return ".info";
                case 0x0c:
                    return ".biz";
                case 0x0d:
                    return ".gov";
                default:
                    return System.Text.Encoding.UTF8.GetString(new[] { charVal });
            }
        }
    }
}