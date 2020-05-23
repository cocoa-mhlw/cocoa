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
        /// Secret key
        /// </summary>
        /// <value>Secret Key</value>
        public string Secret { get; set; }

        /// <summary>
        /// Jump Consistent Hash 
        /// </summary>
        /// <value>Jump Consistent Hash</value>
        public ulong JumpConsistentHash { get; set; }

        /// <summary>
        /// Last notification date and time
        /// </summary>
        public DateTime LastNotificationTime { get; set; }

        public bool Equals(UserDataModel other)
        {
            return UserUuid == other?.UserUuid
                && LastNotificationTime == other?.LastNotificationTime;
        }

        /// <summary>
        /// User Unique ID format UserUUID.(Padding Zero Major).(Padding Zero Minor)
        /// </summary>
        /// <value>User Minor</value>
        public string GetId()
        {
            return UserUuid;
        }

        public int GetJumpHashTimeDifference()
        {
            return JumpHash.JumpConsistentHash(JumpConsistentHash, AppConstants.NumberOfGroup);
        }
    }
}
