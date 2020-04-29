using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    public class ContactModel
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
        /// The splited timespan.
        /// </summary>
        public string KeyTime { get; set; }

        public BeaconModel Beacon1 { get; set; }
        public BeaconModel Beacon2 { get; set; }

    }
}
