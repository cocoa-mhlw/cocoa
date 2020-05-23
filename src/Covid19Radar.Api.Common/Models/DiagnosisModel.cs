using System;
using System.Collections.Generic;
using System.Text;

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
        /// Temporary Exposure Keys
        /// </summary>
        public TemporaryExposureKeyModel[] Keys { get; set; }

    }
}
