using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    class UserDataModel
    {

        /// <summary>
        /// User UUID
        /// </summary>
        /// <value>User UUID</value>
        public string Uuid { get; set; }

        /// <summary>
        /// User Major
        /// </summary>
        /// <value>User Major</value>
        public string Major { get; set; }

        /// <summary>
        /// User Minor
        /// </summary>
        /// <value>User Minor</value>
        public string Minor { get; set; }

        public string GetId()
        {
            return String.Format("{0}.{1}.{2}", Uuid, Major.PadLeft(5, '0'), Minor.PadLeft(5, '0'));
        }
    }
}
