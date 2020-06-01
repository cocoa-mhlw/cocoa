using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Model
{
    public class RegisterResultModel
    {
        /// <summary>
        /// User Internal UUID
        /// </summary>
        /// <value>User Internal UUID</value>
        public string UserUuid { get; set; }

        /// <summary>
        /// API Secret key
        /// </summary>
        /// <value>Secret Key</value>
        public string Secret { get; set; }

        /// <summary>
        /// Jump Consistent Hash   Key
        /// </summary>
        /// <value>Jump Consistent Hash</value>
        public ulong JumpConsistentSeed { get; set; }

    }
}
