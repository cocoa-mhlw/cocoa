﻿/* This Source Code Form is subject to the terms of the Mozilla Public
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                try
                {
                    var valueString = preference.GetString(key, null);
                    var value = DateTime.Parse(valueString);
                    loggerService.EndMethod();
                    return value;
                }
                catch (Exception)
                {
                    loggerService.Error($"Failed to get value of {key}");
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

                var context = Android.App.Application.Context;
                var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                try
                {
                    object value = null;
                    if (defaultValue == null)
                    {
                        value = preference.GetString(key, null);
                        return (T)value;
                    }

                    switch (defaultValue)
                    {
                        case int i:
                            value = preference.GetInt(key, i);
                            break;
                        case long l:
                            value = preference.GetLong(key, l);
                            break;
                        case bool b:
                            value = preference.GetBoolean(key, b);
                            break;
                        case float f:
                            value = preference.GetFloat(key, f);
                            break;
                        case string s:
                            value = preference.GetString(key, s);
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
                    loggerService.Exception($"Failed to get value of {key}", e);
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
                    switch (value)
                    {
                        case int i:
                            editor.PutInt(key, i);
                            break;
                        case long l:
                            editor.PutLong(key, l);
                            break;
                        case bool b:
                            editor.PutBoolean(key, b);
                            break;
                        case float f:
                            editor.PutFloat(key, f);
                            break;
                        case string s:
                            editor.PutString(key, s);
                            break;
                    }
                    result = editor.Commit();
                }

                if (!result)
                {
                    loggerService.Error($"Failed to save value of {key}");
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

                    var context = Android.App.Application.Context;
                    var preference = context.GetSharedPreferences(context.PackageName, Android.Content.FileCreationMode.Private);
                    var editor = preference.Edit();
                    editor.Remove(key).Commit();
                }

                loggerService.EndMethod();
            }
        }
    }
}
