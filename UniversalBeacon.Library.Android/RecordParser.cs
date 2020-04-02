using System;
using System.Collections.Generic;
using System.Linq;
using UniversalBeacon.Library.Core.Interop;

namespace UniversalBeacon.Library
{
    internal class RecordParser
    {
        public static BLERecord[] Parse(byte[] recordData)
        {
            var results = new List<BLERecord>();

            var index = 0;
            while (index < recordData.Length)
            {
                // first byte in the record length
                var length = recordData[index++];

                if (length == 0) break;

                // get the type
                var type = (BLEPacketType)recordData[index];

                // copy off the packetData - the type is part of the length so adjust for that
                var data = recordData.Skip(index + 1).Take(length - 1).ToArray();

                switch (type)
                {
                    case BLEPacketType.Invalid:
                        // zero is invalid
                        break;
                    case BLEPacketType.Flags:
                        results.Add(new BLEFlagRecord(type, data[0]));
                        break;
                    case  BLEPacketType.LocalName:
                        results.Add(new BLENameRecord(type, data));
                        break;
                    case BLEPacketType.ManufacturerData:
                        results.Add(new BLEManufacturerData(type, data));
                        break;
                    case BLEPacketType.ServiceData:
                        results.Add(new BLEAdvertisementDataSection(type, data));
                        break;
                    case BLEPacketType.UUID16List:
                        // swap endianness
                        Array.Reverse(data);
                        results.Add(new BLEGenericRecord(type, data));
                        break;
                    default:
                        results.Add(new BLEGenericRecord(type, data));
                        break;
                }

                index += length;
                /*
                                if (!Enum.IsDefined(typeof(AdvertisementRecordType), type))
                                {
                                    Trace.Message("Advertisment record type not defined: {0}", type);
                                    break;
                                }

                                //data length is length -1 because type takes the first byte
                                byte[] data = new byte[length - 1];
                                Array.Copy(scanRecord, index + 1, data, 0, length - 1);

                                // don't forget that data is little endian so reverse
                                // Supplement to Bluetooth Core Specification 1
                                // NOTE: all relevant devices are already little endian, so this is not necessary for any type except UUIDs
                                //var record = new AdvertisementRecord((AdvertisementRecordType)type, data.Reverse().ToArray());

                                switch ((AdvertisementRecordType)type)
                                {
                                    case AdvertisementRecordType.ServiceDataUuid32Bit:
                                    case AdvertisementRecordType.SsUuids128Bit:
                                    case AdvertisementRecordType.SsUuids16Bit:
                                    case AdvertisementRecordType.SsUuids32Bit:
                                    case AdvertisementRecordType.UuidCom32Bit:
                                    case AdvertisementRecordType.UuidsComplete128Bit:
                                    case AdvertisementRecordType.UuidsComplete16Bit:
                                    case AdvertisementRecordType.UuidsIncomple16Bit:
                                    case AdvertisementRecordType.UuidsIncomplete128Bit:
                                        Array.Reverse(data);
                                        break;
                                }
                                var record = new AdvertisementRecord((AdvertisementRecordType)type, data);

                                Trace.Message(record.ToString());

                                records.Add(record);

                                //Advance
                                index += length;
                            }
                            */
            }
            return results.ToArray();
        }
    }

}
