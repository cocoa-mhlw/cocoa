/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;

namespace Covid19Radar.Common
{
    public interface IDateTimeUtility
    {
        DateTime UtcNow { get; }
    }

    public class DateTimeUtility : IDateTimeUtility
    {
        public static IDateTimeUtility Instance = new DateTimeUtility();

        public DateTimeUtility()
        {
        }

        public DateTime UtcNow => DateTime.UtcNow;
    }
}
