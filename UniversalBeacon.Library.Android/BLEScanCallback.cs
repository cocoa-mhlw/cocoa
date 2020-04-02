using System;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Runtime;
using UniversalBeacon.Library.Core.Interop;

namespace UniversalBeacon.Library
{
    internal class BLEScanCallback : ScanCallback
    {
        public event EventHandler<BLEAdvertisementPacketArgs> OnAdvertisementPacketReceived;

        public override void OnScanFailed([GeneratedEnum] ScanFailure errorCode)
        {
            base.OnScanFailed(errorCode);
        }

        public override void OnScanResult([GeneratedEnum] ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);

            switch (result.Device.Type)
            {
                case BluetoothDeviceType.Le:
                case BluetoothDeviceType.Unknown:
                    try
                    {
                        var p = new BLEAdvertisementPacket
                        {
                            // address will be in the form "D1:36:E6:9D:46:52"
                            BluetoothAddress = result.Device.Address.ToNumericAddress(),    
                            RawSignalStrengthInDBm = (short) result.Rssi,
                            // TODO: probably needs adjustment
                            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(result.TimestampNanos / 1000),
                            // TODO: validate this
                            AdvertisementType = (BLEAdvertisementType) result.ScanRecord.AdvertiseFlags, 
                            Advertisement = new BLEAdvertisement
                            {
                                LocalName = result.ScanRecord.DeviceName
                            }
                        };

                        if (result.ScanRecord.ServiceUuids != null)
                        {
                            foreach(var svc in result.ScanRecord.ServiceUuids)
                            {
                                var guid = new Guid(svc.Uuid.ToString());
                                var data = result.ScanRecord.GetServiceData(svc);

                                p.Advertisement.ServiceUuids.Add(guid);
                            }
                        }

                        var recordData = result.ScanRecord.GetBytes();
                        var rec = RecordParser.Parse(recordData);

                        foreach (var curRec in rec)
                        {
                            if (curRec is BLEManufacturerData md)
                            {
                                p.Advertisement.ManufacturerData.Add(md);
                            }
                            if (curRec is BLEAdvertisementDataSection sd)
                            {
                                p.Advertisement.DataSections.Add(sd);
                            }
                        }
                        OnAdvertisementPacketReceived?.Invoke(this, new BLEAdvertisementPacketArgs(p));
                    }
                    catch (Exception)
                    {
                        // TODO
                    }
                    break;
                default:
                    break;
            }

            // result.Device;
        }
    }
}
