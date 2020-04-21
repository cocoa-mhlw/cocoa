using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    public interface IUserMajorMinor
    {
        /// <summary>
        /// Major - in this app case mapping user id
        /// </summary>
        /// <value>BLE major number</value>
        string UserMajor { get; }
        /// <summary>
        /// MInor - in this app case mapping user id
        /// </summary>
        /// <value>BLE minor number</value>
        string UserMinor { get; }

    }
    public static class IUserMajorMinorExtension
    {
        /// <summary>
        /// They decide left and right
        /// </summary>
        /// <value>User Minor</value>
        public static void SetDecideLeftRight<T>(T b1, T b2, out T r1, out T r2) where T : IUserMajorMinor
        {
            var b1Major = Convert.ToInt32(b1.UserMajor);
            var b1Minor = Convert.ToInt32(b1.UserMinor);
            var b2Major = Convert.ToInt32(b2.UserMajor);
            var b2Minor = Convert.ToInt32(b2.UserMinor);
            if (b1Major > b2Major)
            {
                r1 = b2;
                r2 = b1;
            }
            else if (b1Major < b2Major)
            {
                r1 = b1;
                r2 = b2;
            }
            else if (b1Minor > b2Minor)
            {
                r1 = b2;
                r2 = b1;
            }
            r1 = b1;
            r2 = b2;
        }
    }

}
