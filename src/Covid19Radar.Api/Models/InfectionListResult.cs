using System;
using Newtonsoft.Json;

namespace Covid19Radar.Models
{
    [JsonObject("infectionListResult")]
    public class InfectionListResult
    {
        /// <summary>
        /// Last date and time
        /// </summary>
        [JsonProperty("lastUpdateTime")]
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// list of InfectionItem
        /// </summary>
        [JsonProperty("list")]
        public Item[] List { get; set; }

        [JsonObject("item")]
        public class Item
        {
            /// <summary>
            /// User Major 0 to 65536
            /// </summary>
            /// <value>User Major</value>
            [JsonProperty("major")]
            public string Major { get; set; }

            /// <summary>
            /// User Minor 0 to 65536
            /// </summary>
            /// <value>User Minor</value>
            [JsonProperty("minor")]
            public string Minor { get; set; }

            /// <summary>
            /// Impact start date
            /// </summary>
            [JsonProperty("impactStart")]
            public DateTime ImpactStart { get; set; }

            /// <summary>
            /// Impact end date
            /// </summary>
            [JsonProperty("impactEnd")]
            public DateTime ImpactEnd { get; set; }
        }
    }

}
