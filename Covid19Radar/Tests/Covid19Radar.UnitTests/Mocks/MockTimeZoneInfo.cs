// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Reflection;

namespace Covid19Radar.UnitTests.Mocks
{
    public class MockTimeZoneInfo
    {
        public MockTimeZoneInfo()
        {
        }

        public static void SetJstLocalTimeZone() => SetLocalTimeZone(JstTimeZoneInfo());

        public static void ClearLocalTimeZone()
        {
            TimeZoneInfo.ClearCachedData();
        }

        public static bool IsJst()
        {
            return TimeZoneInfo.Local.BaseUtcOffset.Hours == 9 && TimeZoneInfo.Local.BaseUtcOffset.Minutes == 0;
        }

        private static void SetLocalTimeZone(TimeZoneInfo timeZoneInfo)
        {
            FieldInfo cachedDataFieldInfo = typeof(TimeZoneInfo).GetField(
                "s_cachedData",
                BindingFlags.NonPublic | BindingFlags.Static);
            object cachedData = cachedDataFieldInfo.GetValue(null);

            FieldInfo localTimeZoneFieldInfo = cachedData.GetType().GetField(
                "_localTimeZone",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Instance);
            localTimeZoneFieldInfo.SetValue(cachedData, timeZoneInfo);
        }

        private static TimeZoneInfo JstTimeZoneInfo()
        {
            // iOS/Android/Unix
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
            }
            catch (TimeZoneNotFoundException)
            {
                // Not iOS/Android/Unix
            }

            // Windows
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                // Not Windows
            }

            // Emergency fallback
            return TimeZoneInfo.CreateCustomTimeZone("JST", new TimeSpan(9, 0, 0), "(GMT+09:00) JST", "JST");
        }
    }
}

