using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Covid19Radar.Common;
using Newtonsoft.Json.Converters;

namespace Covid19Radar.Models
{
    public class UserResultModel
    {
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
        /// set SserStatus
        /// </summary>
        /// <param name="s">UserStatus</param>
        public void SetStatus(UserStatus s)
        {
            _UserStatus = s;
        }

    }
}
