/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Services
{
    public interface IPreferencesService
    {
        T GetValue<T>(string key, T defaultValue);
        void SetValue<T>(string key, T value);
        void RemoveValue(string key);
        bool ContainsKey(string key);
    }
}
