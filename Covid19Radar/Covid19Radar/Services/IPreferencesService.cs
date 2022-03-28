/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Services
{
    public interface IPreferencesService
    {
        int GetIntValue(string key, int defaultValue);
        long GetLongValue(string key, long defaultValue);
        float GetFloatValue(string key, float defaultValue);
        string GetStringValue(string key, string defaultValue);
        bool GetBoolValue(string key, bool defaultValue);

        void SetIntValue(string key, int value);
        void SetLongValue(string key, long value);
        void SetFloatValue(string key, float value);
        void SetStringValue(string key, string value);
        void SetBoolValue(string key, bool value);

        void RemoveValue(string key);
        bool ContainsKey(string key);
    }
}
