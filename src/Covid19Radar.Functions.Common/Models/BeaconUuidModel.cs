
using Covid19Radar.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Covid19Radar.Models
{
    public sealed class BeaconUuidModel
    {
        public BeaconUuidModel(DateTime now)
        {
            id = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, TimeSpan.Zero).ToString("yyyyMMddHHmm");
            BeaconUuid = Guid.NewGuid().ToString(); 
            CreateTime = now;
            EndTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(2);
        }

        /// <summary>
        /// for Cosmos DB id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Beacon Uuid
        /// </summary>
        public string BeaconUuid { get; set; }
        /// <summary>
        /// created timestamp UTC
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Reloading Time UTC
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}
