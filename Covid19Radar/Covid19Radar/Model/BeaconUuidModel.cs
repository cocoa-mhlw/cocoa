
using System;
using System.Collections.Generic;
using System.Linq;

namespace Covid19Radar.Models
{
    public sealed class BeaconUuidModel
    {
        public BeaconUuidModel()
        {
        }

        /// <summary>
        /// for Cosmos DB id
        /// </summary>
        public string id;
        /// <summary>
        /// Beacon Uuid
        /// </summary>
        public string BeaconUuid;
        /// <summary>
        /// created timestamp UTC
        /// </summary>
        public DateTime CreateTime;
        /// <summary>
        /// Reloading Time UTC
        /// </summary>
        public DateTime EndTime;
    }
}
