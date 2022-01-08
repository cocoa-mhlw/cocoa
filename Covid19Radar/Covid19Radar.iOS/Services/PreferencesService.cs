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

        public DateTime? GetDateTime(string key)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}, type={typeof(DateTime)}");

                if (!ContainsKey(key))
                {
                    loggerService.Info($"{key} is not contained.");
                    loggerService.EndMethod();
                    return null;
                }

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                try
                {
                    var valueString = userDefaults.StringForKey(key);
                    var value = DateTime.Parse(valueString);
                    loggerService.EndMethod();
                    return value;
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}.");
                    loggerService.EndMethod();
                    return null;
                }
            }
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}, type={typeof(T)}");

                if (!ContainsKey(key))
                {
                    loggerService.Info($"{key} is not contained.");
                    loggerService.EndMethod();
                    return defaultValue;
                }

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                try
                {
                    object value = null;
                    if (defaultValue == null)
                    {
                        value = userDefaults.StringForKey(key);
                        return (T)value;
                    }

                    switch (defaultValue)
                    {
                        case int i:
                            value = (int)userDefaults.IntForKey(key);
                            break;
                        case long l:
                            value = (long)userDefaults.IntForKey(key);
                            break;
                        case bool b:
                            value = userDefaults.BoolForKey(key);
                            break;
                        case float f:
                            value = userDefaults.FloatForKey(key);
                            break;
                        case string s:
                            value = userDefaults.StringForKey(key);
                            break;
                        default:
                            loggerService.Info("Type is not supported.");
                            value = defaultValue;
                            break;
                    }
                    loggerService.EndMethod();
                    return (T)value;
                }
                catch (Exception e)
                {
                    loggerService.Exception($"Failed to get value of {key}.", e);
                    loggerService.EndMethod();
                    return defaultValue;
                }
            }
        }

        public void SetValue<T>(string key, T value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}, type={typeof(T)}");

                var userDefaults = NSUserDefaults.StandardUserDefaults;
                bool result;

                if (ContainsKey(key) && value == null)
                {
                    userDefaults.RemoveObject(key);
                    result = userDefaults.Synchronize();
                }
                else
                {
                    switch (value)
                    {
                        case int i:
                            userDefaults.SetInt(i, key);
                            break;
                        case long l:
                            userDefaults.SetInt((nint)l, key);
                            break;
                        case bool b:
                            userDefaults.SetBool(b, key);
                            break;
                        case float f:
                            userDefaults.SetFloat(f, key);
                            break;
                        case string s:
                            userDefaults.SetString(s, key);
                            break;
                    }

                    result = userDefaults.Synchronize();
                }

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}.");
                }

                loggerService.EndMethod();
            }
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
    }
}
