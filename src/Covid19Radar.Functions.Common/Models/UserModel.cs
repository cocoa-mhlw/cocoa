using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Covid19Radar.Common;
using Newtonsoft.Json.Converters;

namespace Covid19Radar.Models
{
    public class UserModel : IUser
    {
        /// <summary>
        /// for CosmosDB id
        /// </summary>
        public string id { get => this.GetId(); }
        /// <summary>
        /// for CosmosDB PartitionKey
        /// </summary>
        public string PartitionKey { get => this.GetId(); }
        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        public string UserUuid { get; set; }


        /// <summary>
        /// Protected Secret key
        /// </summary>
        /// <value>Protected Secret Key</value>
        public string ProtectSecret { get; set; }

        /// <summary>
        /// ETAG
        /// </summary>
        public string _etag { get; set; }

        /// <summary>
        ///  Status Contacted,OnSet,Suspected,Inspection,Infection
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
        /// set SserStatus
        /// </summary>
        /// <param name="s">UserStatus</param>
        public void SetStatus(UserStatus s)
        {
            _UserStatus = s;
        }
    }
}
