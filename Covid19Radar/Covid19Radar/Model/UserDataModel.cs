using Covid19Radar.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Model
{
    public class UserDataModel: IEquatable<UserDataModel>
    {

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
        public UserStatus UserStatus { get; set; }

        public bool Equals(UserDataModel other)
        {
            return UserUuid == other?.UserUuid
                && Major == other?.Major
                && Minor == other?.Minor
                && UserStatus == other?.UserStatus;
        }

        /// <summary>
        /// User Unique ID format UserUUID.(Padding Zero Major).(Padding Zero Minor)
        /// </summary>
        /// <value>User Minor</value>
        public string GetId()
        {
            return String.Format("{0}.{1}.{2}", UserUuid, Major.PadLeft(5, '0'), Minor.PadLeft(5, '0'));
        }

        public int GetJumpHashTimeDifference()
        {
            return JumpHash.JumpConsistentHash(Convert.ToUInt64(Major) + Convert.ToUInt64(Minor), AppConstants.NumberOfGroup);
        }
    }
}
