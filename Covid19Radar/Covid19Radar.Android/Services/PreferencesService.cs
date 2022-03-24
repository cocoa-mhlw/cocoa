/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Droid.Services
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
            var context = Android.App.Application.Context;
            var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
            return preference.Contains(key);
        }

        public void RemoveValue(string key)
        {
            lock (this)
            {
                loggerService.StartMethod();

                if (ContainsKey(key))
                {
                    loggerService.Info($"key={key}");

                    var context = Android.App.Application.Context;
                    var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                    var editor = preference.Edit();
                    editor.Remove(key).Commit();
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                try
                {
                    loggerService.EndMethod();
                    return preference.GetInt(key, defaultValue);
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}");
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                try
                {
                    loggerService.EndMethod();
                    return preference.GetLong(key, defaultValue);
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}");
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                try
                {
                    loggerService.EndMethod();
                    return preference.GetFloat(key, defaultValue);
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}");
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                try
                {
                    loggerService.EndMethod();
                    return preference.GetString(key, defaultValue);
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}");
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                try
                {
                    loggerService.EndMethod();
                    return preference.GetBoolean(key, defaultValue);
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}");
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                var editor = preference.Edit();

                editor.PutInt(key, value);
                bool result = editor.Commit();

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}");
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                var editor = preference.Edit();

                editor.PutLong(key, value);
                bool result = editor.Commit();

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}");
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                var editor = preference.Edit();

                editor.PutFloat(key, value);
                bool result = editor.Commit();

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}");
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                var editor = preference.Edit();

                bool result;
                if (ContainsKey(key) && value == null)
                {
                    editor.Remove(key);
                    result = editor.Commit();
                }
                else
                {
                    editor.PutString(key, value);
                    result = editor.Commit();
                }

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}");
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                var editor = preference.Edit();

                editor.PutBoolean(key, value);
                bool result = editor.Commit();

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}");
                }

                loggerService.EndMethod();
            }
        }
    }
}
