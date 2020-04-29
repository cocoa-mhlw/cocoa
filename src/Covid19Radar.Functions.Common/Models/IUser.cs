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
        string Major { get; }

        /// <summary>
        /// User Minor 0 to 65536
        /// </summary>
        /// <value>User Minor</value>
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
            return string.Format("{0}.{1}.{2}", user.UserUuid, user.Major.PadLeft(5, '0'), user.Minor.PadLeft(5, '0'));
        }
    }
}
