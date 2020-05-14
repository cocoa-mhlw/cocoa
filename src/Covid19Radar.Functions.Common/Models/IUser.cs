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

    }

    public static class IUserExtension
    {
        /// <summary>
        /// User Unique ID format UserUUID.(Padding Zero Major).(Padding Zero Minor)
        /// </summary>
        /// <value>User Minor</value>
        public static string GetId(this IUser user)
        {
            return user.UserUuid;
        }
    }
}
