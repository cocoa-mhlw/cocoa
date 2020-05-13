using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    public interface IUser
    {
        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        string UserUuid { get; }

        /// <summary>
        /// User Major 0 to 65536
        /// </summary>
        /// <value>User Major</value>
        [Obsolete]
        string Major { get; }

        /// <summary>
        /// User Minor 0 to 65536
        /// </summary>
        /// <value>User Minor</value>
        [Obsolete]
        string Minor { get; }

    }

    public static class IUserExtension
    {
        /// <summary>
        /// User Unique ID format UserUUID.(Padding Zero Major).(Padding Zero Minor)
        /// </summary>
        /// <value>User Minor</value>
        public static string GetId(this IUser user)
        {
            if (string.IsNullOrEmpty(user.Major) && string.IsNullOrEmpty(user.Minor))
            {
                return user.UserUuid;
            }
            return string.Format("{0}.{1}.{2}", user.UserUuid, user.Major.PadLeft(5, '0'), user.Minor.PadLeft(5, '0'));
        }
    }
}
