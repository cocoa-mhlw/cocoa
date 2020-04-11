using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Covid19Radar.Common;
using Newtonsoft.Json.Converters;

namespace Covid19Radar.Models
{
    public class UserDataModel
    {
        /// <summary>
        /// for CosmosDB id
        /// </summary>
        public string id { get => GetId(); }
        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        public string UserUuid { get; set; }

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
        ///  Status Contactd,OnSet,Suspected,Inspection,Infection
        /// </summary>
        /// <value></value>
        private UserStatus _UserStatus;
        public string UserStatus
        {
            get
            {
                return Enum.GetName(typeof(Covid19Radar.Common.UserStatus), _UserStatus);
            }
            set
            {
                _UserStatus = Enum.Parse<Covid19Radar.Common.UserStatus>(value);
            }
        }

        /// <summary>
        /// User Unique ID format UserUUID.(Padding Zero Major).(Padding Zero Minor)
        /// </summary>
        /// <value>User Minor</value>
        public string GetId()
        {
            return String.Format("{0}.{1}.{2}", UserUuid, Major.PadLeft(5, '0'), Minor.PadLeft(5, '0'));
        }

        /// <summary>
        /// set SserStatus
        /// </summary>
        /// <param name="s">UserStatus</param>
        public void SetStatus(UserStatus s)
        {
            _UserStatus = s;
        }

    }
}
