using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    public class InfectionListResult
    {
        /// <summary>
        /// Last date and time
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// list of InfectionItem
        /// </summary>
        public Item[] List { get; set; }

        public class Item
        {
            /// <summary>
            /// User Major 0 to 65536
            /// </summary>
            /// <value>User Major</value>
            public string Major { get; set; }

            /// <summary>
            /// User Minor 0 to 65536
            /// </summary>
            /// <value>User Minor</value>
            public string Minor { get; set; }

            /// <summary>
            /// Impact start date
            /// </summary>
            public DateTime ImpactStart { get; set; }

            /// <summary>
            /// Impact end date
            /// </summary>
            public DateTime ImpactEnd { get; set; }
        }
    }

}
