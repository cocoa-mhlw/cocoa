// Copyright 2015 - 2017 Andreas Jakl. All rights reserved. 
// https://github.com/andijakl/universal-beacon 
// 
// Based on the Eddystone specification by Google, 
// available under Apache License, Version 2.0 from
// https://github.com/google/eddystone
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
//    http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UniversalBeacon.Library.Core.Entities
{
    /// <summary>
    /// An Eddystone URL frame, according to the Google Specification from
    /// https://github.com/google/eddystone/tree/master/eddystone-url
    /// </summary>
    public class UrlEddystoneFrame : BeaconFrameBase
    {
        private sbyte _rangingData;

        /// <summary>
        /// Tx power level - the received power level at 0 m, in dBm.
        /// Values range from -100 to +20 dBM, with a resolution of 1 dBm.
        /// Signed 8 bit integer according to:
        /// https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.tx_power_level.xml
        /// </summary>
        public sbyte RangingData
        {
            get => _rangingData;
            set
            {
                if (_rangingData == value) return;
                _rangingData = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        private string _completeUrl;
        /// <summary>
        /// The decoded complete URL contained in this advertisement frame.
        /// </summary>
        public string CompleteUrl
        {
            get => _completeUrl;
            set
            {
                if (_completeUrl == value) return;
                _completeUrl = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The encoded URL may only contain graphic printable characters of the US-ASCII coded character set.
        /// Other byte values contained in the URL that are found in this dictionary are expanded to the
        /// complete URL, to save bytes in the overall encoded URL.
        /// </summary>
        private static readonly Dictionary<byte, string> UrlCodeDictionary = new Dictionary<byte, string>
        {
            {0, ".com/"},
            {1, ".org/"},
            {2, ".edu/"},
            {3, ".net/"},
            {4, ".info/"},
            {5, ".biz/"},
            {6, ".gov/"},
            {7, ".com"},
            {8, ".org"},
            {9, ".edu"},
            {10, ".net"},
            {11, ".info"},
            {12, ".biz"},
            {13, ".gov"},
        };

        /// <summary>
        /// The first byte of the URL data contains the URL scheme.
        /// This byte is mandatory and only the four values contained in this dictionary
        /// are currently allowed in the specification.
        /// </summary>
        private static readonly Dictionary<byte, string> UrlSchemeDictionary = new Dictionary<byte, string>
        {
            {0, "http://www."},
            {1, "https://www."},
            {2, "http://"},
            {3, "https://"}
        };

        /// <summary>
        /// Create new URL Eddystone frame based on the provided data.
        /// </summary>
        /// <param name="rangingData">Ranging data (Tx power level) - see property documentation.</param>
        /// <param name="completeUrl">URL to use for the frame.</param>
        public UrlEddystoneFrame(sbyte rangingData, string completeUrl)
        {
            _rangingData = rangingData;
            _completeUrl = completeUrl;
            UpdatePayload();
        }

        /// <summary>
        /// Create new instance of the URL Eddystone frame based on the provided payload.
        /// Parses the payload and initializes the instance.
        /// </summary>
        /// <param name="payload">Payload to parse for this frame type.</param>
        public UrlEddystoneFrame(byte[] payload) : base(payload)
        {
            ParsePayload();
        }

        /// <summary>
        /// Parse the current payload into the properties exposed by this class.
        /// Has to be called if manually modifying the raw payload.
        /// </summary>
        public void ParsePayload()
        {
            if (!IsValid()) return;

            // Ranging data
            var newRangingData = (sbyte)Payload[BeaconFrameHelper.EddystoneHeaderSize];
            if (newRangingData != RangingData)
            {
                _rangingData = newRangingData;
                OnPropertyChanged(nameof(RangingData));
            }

            // URL Scheme prefix (1 byte)
            var urlSchemePrefix = Payload[BeaconFrameHelper.EddystoneHeaderSize + 1];
            
            // Decode complete URL
            var newCompleteUrl = UrlSchemePrefixAsString(urlSchemePrefix) + DecodeUrl(Payload, BeaconFrameHelper.EddystoneHeaderSize + 2);
            if (newCompleteUrl != CompleteUrl)
            {
                _completeUrl = newCompleteUrl;
                OnPropertyChanged(nameof(CompleteUrl));
            }

            //Debug.WriteLine("Eddystone URL Frame: Url = " + CompleteUrl);
        }

        /// <summary>
        /// Update the raw payload when properties have changed.
        /// </summary>
        private void UpdatePayload()
        {
            if (string.IsNullOrEmpty(CompleteUrl))
            {
                _payload = null;
                return;
            }
            var urlSchemeByte = EncodeUrlScheme(CompleteUrl);
            // Check if the URL starts with a valid header (only the defined ones are allowed)
            if (urlSchemeByte == null)
            {
                _payload = null;
                return;
            }

            var header = BeaconFrameHelper.CreateEddystoneHeader(BeaconFrameHelper.EddystoneFrameType.UrlFrameType);
            using (var ms = new MemoryStream())
            {
                // Frame header
                ms.Write(header, 0, header.Length);
                // Ranging data
                ms.WriteByte((byte)RangingData);
                // URL scheme byte
                ms.WriteByte((byte)urlSchemeByte);
                // Encoded URL
                EncodeUrlToStream(CompleteUrl, UrlSchemeDictionary[(byte) urlSchemeByte].Length, ms);
                // Save to payload (to direct array to prevent re-parsing and a potential endless loop of updating and parsing)
                _payload = ms.ToArray();
            }
        }

        /// <summary>
        /// Return the URL scheme (starting characters of the URL) as encoded byte.
        /// Returns null if the scheme is not found in the definition, which is not
        /// valid for the Eddystone URL format.
        /// </summary>
        /// <param name="url">URL to analyze.</param>
        /// <returns>Byte to use for the encoded URL.</returns>
        private static byte? EncodeUrlScheme(string url)
        {
            foreach (var curScheme in UrlSchemeDictionary)
            {
                if (url.StartsWith(curScheme.Value, StringComparison.OrdinalIgnoreCase))
                {
                    return curScheme.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Parse the provided URL starting at the provided position 
        /// (after the URL scheme) and write the encoded data into the provided stream.
        /// </summary>
        /// <param name="url">URL to encode.</param>
        /// <param name="pos">Start position in the URL string, after the URL scheme.</param>
        /// <param name="ms">Memory stream to encode the data to.</param>
        private void EncodeUrlToStream(string url, int pos, Stream ms)
        {
            // Encode the rest of the URL string
            while(pos < url.Length)
            {
                var code = FindUrlCode(url.Substring(pos));
                if (code == null)
                {
                    // Didn't find URL code at this position
                    var curCharByte = Encoding.ASCII.GetBytes(new[] {url[pos]});
                    ms.Write(curCharByte, 0, curCharByte.Length);
                    pos++;
                }
                else
                {
                    // Write URL code
                    ms.WriteByte((byte)code);
                    pos += UrlCodeDictionary[(byte) code].Length;
                }
            }
        }

        /// <summary>
        /// Check the if the provided URL string starts with one of the
        /// URL codes from the dictionary. 
        /// </summary>
        /// <param name="url">URL where the beginning should be analyzed.</param>
        /// <returns>Returns the encoded byte value if the beginning of the URL
        /// string is present in the dictionary, or null if not.</returns>
        private byte? FindUrlCode(string url)
        {
            var dictPos = -1;
            var expansionLength = 0;

            for (var i = 0; i < UrlCodeDictionary.Count; i++)
            {
                if (UrlCodeDictionary[(byte)i].Length > expansionLength)
                {
                    if (url.StartsWith(UrlCodeDictionary[(byte)i]))
                    {
                        expansionLength = UrlCodeDictionary[(byte)i].Length;
                        dictPos = i;
                    }
                }
            }
            
            if (dictPos >= 0)
                return (byte)dictPos;
            return null;
        }

        /// <summary>
        /// Decode the provided URL into a normal string.
        /// Does not decode the URL scheme, use the extra method for that.
        /// </summary>
        /// <param name="rawUrl">Raw encoded URL to expand into a standard string.</param>
        /// <param name="startAtArrayPos">Start analyzing the byte array at this position
        /// (after the scheme)</param>
        /// <returns>Decoded URL.</returns>
        private static string DecodeUrl(byte[] rawUrl, int startAtArrayPos)
        {
            if (rawUrl.Length < startAtArrayPos) return null;
            var sb = new StringBuilder(rawUrl.Length);
            for(var i = startAtArrayPos; i < rawUrl.Length; i++)
            {
                sb.Append(UrlCodeDictionary.ContainsKey(rawUrl[i])
                    ? UrlCodeDictionary[rawUrl[i]]
                    : Encoding.UTF8.GetString(new[] { rawUrl[i] }));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get the URL scheme as string for the specified scheme prefix byte.
        /// </summary>
        /// <param name="urlSchemePrefix">URL prefix byte to analyze.</param>
        /// <returns>Decoded URL scheme as string.</returns>
        public string UrlSchemePrefixAsString(byte urlSchemePrefix)
        {
            return UrlSchemeDictionary.ContainsKey(urlSchemePrefix) ? UrlSchemeDictionary[urlSchemePrefix] : null;
        }

        /// <summary>
        /// Update the information stored in this frame with the information from the other frame.
        /// Useful for example when binding the UI to beacon information, as this will emit
        /// property changed notifications whenever a value changes - which would not be possible if
        /// you would overwrite the whole frame.
        /// </summary>
        /// <param name="otherFrame">Frame to use as source for updating the information in this beacon
        /// frame.</param>
        public override void Update(BeaconFrameBase otherFrame)
        {
            base.Update(otherFrame);
            ParsePayload();
        }

        /// <summary>
        /// Check if the contents of this frame are generally valid.
        /// Does not currently perform a deep analysis, but checks the header as well
        /// as the frame length.
        /// </summary>
        /// <returns>True if the frame is a valid Eddystone URL frame.</returns>
        public override bool IsValid()
        {
            if (!base.IsValid()) return false;

            // 2 bytes ID: AA FE
            // 1 byte frame type
            if (!Payload.IsEddystoneFrameType()) return false;
            
            // Check if the frame type is correct for URL
            var eddystoneFrameType = Payload.GetEddystoneFrameType();
            if (eddystoneFrameType == null || eddystoneFrameType !=
                BeaconFrameHelper.EddystoneFrameType.UrlFrameType) return false;

            // 1 byte ranging data
            // 1 byte url scheme prefix
            return Payload.Length >= BeaconFrameHelper.EddystoneHeaderSize + 2;
        }
    }
}
