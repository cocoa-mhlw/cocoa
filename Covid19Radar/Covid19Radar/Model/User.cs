using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Model
{
    class UserData
    {

        public UserData()
        {
            // Default Value
            this.Uuid = Guid.NewGuid();
            this.IsRegistered = false;
        }
        /// <summary>
        /// User UUID
        /// </summary>
        /// <value>User UUID</value>
        public Guid Uuid { get; set; }

        /// <summary>
        ///Registered State 
        /// </summary>
        /// <value>r</value>
        public bool IsRegistered { get; set; }
    }
}
