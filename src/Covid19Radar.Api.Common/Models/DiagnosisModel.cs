/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Api.Models
{
    public class DiagnosisModel
    {
        /// <summary>
        /// for Cosmos DB
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// PartitionKey
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// User UUID
        /// </summary>
        /// <value>User UUID</value>
        public string UserUuid { get; set; }

        /// <summary>
        /// Submission number
        /// </summary>
        public string SubmissionNumber { get; set; }

        /// <summary>
        /// Public approval
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// timestamp unit time seconds
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Temporary Exposure Keys
        /// </summary>
        public TemporaryExposureKeyModel[] Keys { get; set; }

    }
}
