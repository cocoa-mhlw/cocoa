/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using Covid19Radar.Api.Common;
using Newtonsoft.Json;

namespace Covid19Radar.Api.Models
{
    public class V1EventLogSubmissionParameter : IDeviceVerification
    {
        [JsonProperty("idempotencyKey")]
        public string IdempotencyKey { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("appPackageName")]
        public string AppPackageName { get; set; }

        [JsonProperty("deviceVerificationPayload")]
        public string DeviceVerificationPayload { get; set; }

        [JsonProperty("eventLogs")]
        public EventLog[] EventLogs { get; set; }

        #region Apple Device Check

        [JsonIgnore]
        public string DeviceToken
        {
            get => DeviceVerificationPayload;
        }
        

        [JsonIgnore]
        public string TransactionIdSeed
        {
            get => string.Join("|", AppPackageName, KeysTextForDeviceVerification);
        }

        #endregion

        #region Android SafetyNet Attestation API

        [JsonIgnore]
        public string JwsPayload
        {
            get => DeviceVerificationPayload;
        }
           

        [JsonIgnore]
        public string ClearText
        {
            get => string.Join("|", AppPackageName, KeysTextForDeviceVerification);
        }
            

        private string KeysTextForDeviceVerification
        {
            get => string.Join(",", EventLogs.Select(el => el.ClearText));
        }
            

        #endregion
    }

    public class EventLog
    {
        [JsonProperty("hasConsent")]
        public bool HasConsent { get; set; }

        [JsonProperty("epoch")]
        public long Epoch { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("subtype")]
        public string Subtype { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonIgnore]
        public string Timestamp
        {
            get => DateTime.UnixEpoch.AddSeconds(Epoch).ToString(Constants.FORMAT_TIMESTAMP);
        }

        public string ClearText
        {
            get => string.Join(".", HasConsent, Epoch, Type, Subtype, Content);
        }
        
    }
}
