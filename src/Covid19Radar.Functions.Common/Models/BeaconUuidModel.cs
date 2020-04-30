
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
        public readonly string id;
        /// <summary>
        /// Beacon Uuid
        /// </summary>
        public readonly string BeaconUuid;
        /// <summary>
        /// created timestamp UTC
        /// </summary>
        public readonly DateTime CreateTime;
        /// <summary>
        /// Reloading Time UTC
        /// </summary>
        public readonly DateTime EndTime;
    }
}
