/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Api.Models
{
    public class EventLogModel
    {
        /// <summary>
        /// for Cosmos DB
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// PartitionKey
        /// </summary>
        public string PartitionKey { get; set; }

        public bool HasConsent { get; }

        public long Epoch { get; }

        public string Type { get; }

        public string Subtype { get; }

        public dynamic Content { get; }

        public string Timestamp { get; }

        public ulong Created { get; set; }

        public EventLogModel(
            bool hasContent,
            long epoch,
            string type,
            string subtype,
            dynamic content,
            string timestamp
            )
        {
            HasConsent = hasContent;
            Epoch = epoch;
            Type = type;
            Subtype = subtype;
            Content = content;
            Timestamp = timestamp;
        }
    }
}
