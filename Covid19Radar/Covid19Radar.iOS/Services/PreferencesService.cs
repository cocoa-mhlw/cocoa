/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Foundation;

namespace Covid19Radar.iOS.Services
{
    public class PreferencesService : IPreferencesService
    {
        private readonly ILoggerService loggerService;

        public PreferencesService(ILoggerService loggerService)
        {
            this.loggerService = loggerService;
        }

        public bool ContainsKey(string key)
        {
            var userDefaults = NSUserDefaults.StandardUserDefaults;
            return userDefaults[key] != null;
        }

        public void RemoveValue(string key)
        {
            lock (this)
            {
                loggerService.StartMethod();

                if (ContainsKey(key))
                {
                    loggerService.Info($"key={key}");

                    var userDefaults = NSUserDefaults.StandardUserDefaults;
                    userDefaults.RemoveObject(key);
                }

                loggerService.EndMethod();
            }
        }

        public int GetIntValue(string key, int defaultValue)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                if (!ContainsKey(key))
                {
                    loggerService.Info($"{key} is not contained.");
                    loggerService.EndMethod();
                    return defaultValue;
                }

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                try
                {
                    loggerService.EndMethod();
                    return (int)userDefaults.IntForKey(key);
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}.");
                    loggerService.EndMethod();
                    return defaultValue;
                }
            }
        }

        public long GetLongValue(string key, long defaultValue)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                if (!ContainsKey(key))
                {
                    loggerService.Info($"{key} is not contained.");
                    loggerService.EndMethod();
                    return defaultValue;
                }

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                try
                {
                    loggerService.EndMethod();
                    return userDefaults.IntForKey(key);
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}.");
                    loggerService.EndMethod();
                    return defaultValue;
                }
            }
        }

        public float GetFloatValue(string key, float defaultValue)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                if (!ContainsKey(key))
                {
                    loggerService.Info($"{key} is not contained.");
                    loggerService.EndMethod();
                    return defaultValue;
                }

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                try
                {
                    loggerService.EndMethod();
                    return userDefaults.FloatForKey(key);
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}.");
                    loggerService.EndMethod();
                    return defaultValue;
                }
            }
        }

        public string GetStringValue(string key, string defaultValue)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                if (!ContainsKey(key))
                {
                    loggerService.Info($"{key} is not contained.");
                    loggerService.EndMethod();
                    return defaultValue;
                }

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                try
                {
                    loggerService.EndMethod();
                    return userDefaults.StringForKey(key);
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}.");
                    loggerService.EndMethod();
                    return defaultValue;
                }
            }
        }

        public bool GetBoolValue(string key, bool defaultValue)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                if (!ContainsKey(key))
                {
                    loggerService.Info($"{key} is not contained.");
                    loggerService.EndMethod();
                    return defaultValue;
                }

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                try
                {
                    loggerService.EndMethod();
                    return userDefaults.BoolForKey(key);
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}.");
                    loggerService.EndMethod();
                    return defaultValue;
                }
            }
        }

        public void SetIntValue(string key, int value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                userDefaults.SetInt(value, key);
                bool result = userDefaults.Synchronize();

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}.");
                }

                loggerService.EndMethod();
            }
        }

        public void SetLongValue(string key, long value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                userDefaults.SetInt((nint)value, key);
                bool result = userDefaults.Synchronize();

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}.");
                }

                loggerService.EndMethod();
            }
        }

        public void SetFloatValue(string key, float value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                userDefaults.SetFloat(value, key);
                bool result = userDefaults.Synchronize();

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}.");
                }

                loggerService.EndMethod();
            }
        }

        public void SetStringValue(string key, string value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                bool result;

                if (ContainsKey(key) && value == null)
                {
                    userDefaults.RemoveObject(key);
                    result = userDefaults.Synchronize();
                }
                else
                {
                    userDefaults.SetString(value, key);
                    result = userDefaults.Synchronize();
                }

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}.");
                }

                loggerService.EndMethod();
            }
        }

        public void SetBoolValue(string key, bool value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                userDefaults.SetBool(value, key);
                bool result = userDefaults.Synchronize();

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}.");
                }

                loggerService.EndMethod();
            }
        }
    }
}
