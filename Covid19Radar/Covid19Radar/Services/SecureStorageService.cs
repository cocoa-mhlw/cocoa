/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public interface ISecureStorageService
    {
        int GetIntValue(string key, int defaultValue);
        long GetLongValue(string key, long defaultValue);
        float GetFloatValue(string key, float defaultValue);
        string GetStringValue(string key, string defaultValue);
        bool GetBoolValue(string key, bool defaultValue);
        double GetDoubleValue(string key, double defaultValue);

        void SetIntValue(string key, int value);
        void SetLongValue(string key, long value);
        void SetFloatValue(string key, float value);
        void SetStringValue(string key, string value);
        void SetBoolValue(string key, bool value);
        void SetDoubleValue(string key, double value);

        bool ContainsKey(string key);
        void RemoveValue(string key);
    }

    public interface ISecureStorageDependencyService
    {
        bool ContainsKey(string key);
        byte[] GetBytes(string key);
        void SetBytes(string key, byte[] bytes);
        void Remove(string key);
    }

    public class SecureStorageService : ISecureStorageService
    {
        private readonly ILoggerService loggerService;
        private readonly ISecureStorageDependencyService secureStorageDependencyService;

        public SecureStorageService(ILoggerService loggerService, ISecureStorageDependencyService secureStorageDependencyService)
        {
            this.loggerService = loggerService;
            this.secureStorageDependencyService = secureStorageDependencyService;
        }

        public bool ContainsKey(string key)
        {
            var contains = false;
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");
                try
                {
                    contains = secureStorageDependencyService.ContainsKey(key);
                    loggerService.Info($"contains={contains}");
                }
                catch (Exception ex)
                {
                    loggerService.Exception("Failed existance confirmation.", ex);
                }
                finally
                {
                    loggerService.EndMethod();
                }
            }
            return contains;
        }

        private T GetValue<T>(string key, T defaultValue = default)
        {
            object result = defaultValue;
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");
                try
                {
                    if (secureStorageDependencyService.ContainsKey(key))
                    {
                        var bytes = secureStorageDependencyService.GetBytes(key);
                        if (typeof(T) == typeof(int) && bytes.Length == sizeof(int))
                        {
                            result = BitConverter.ToInt32(bytes);
                        }
                        else if (typeof(T) == typeof(long) && bytes.Length == sizeof(long))
                        {
                            result = BitConverter.ToInt64(bytes);
                        }
                        else if (typeof(T) == typeof(bool) && bytes.Length == sizeof(bool))
                        {
                            result = BitConverter.ToBoolean(bytes);
                        }
                        else if (typeof(T) == typeof(float) && bytes.Length == sizeof(float))
                        {
                            result = BitConverter.ToSingle(bytes);
                        }
                        else if (typeof(T) == typeof(double) && bytes.Length == sizeof(double))
                        {
                            result = BitConverter.ToDouble(bytes);
                        }
                        else if (typeof(T) == typeof(string))
                        {
                            result = System.Text.Encoding.UTF8.GetString(bytes);
                        }
                        else
                        {
                            throw new InvalidOperationException("Type is not supported.");
                        }
                    }
                    else
                    {
                        loggerService.Info("Value not found.");
                    }
                }
                catch (Exception ex)
                {
                    loggerService.Exception("Failed to get from secure storage.", ex);
                }
                finally
                {
                    loggerService.EndMethod();
                }
            }

            return (T)result;
        }

        private void SetValue<T>(string key, T value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                try
                {
                    byte[] bytes;
                    if (typeof(T) == typeof(int))
                    {
                        bytes = BitConverter.GetBytes((int)(object)value);
                    }
                    else if (typeof(T) == typeof(long))
                    {
                        bytes = BitConverter.GetBytes((long)(object)value);
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        bytes = BitConverter.GetBytes((bool)(object)value);
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        bytes = BitConverter.GetBytes((float)(object)value);
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        bytes = BitConverter.GetBytes((double)(object)value);
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        bytes = System.Text.Encoding.UTF8.GetBytes((string)(object)value);
                    }
                    else
                    {
                        throw new InvalidOperationException("Type is not supported.");
                    }

                    secureStorageDependencyService.SetBytes(key, bytes);
                }
                catch (Exception ex)
                {
                    loggerService.Exception("Failed to set to secure storage.", ex);
                }
                finally
                {
                    loggerService.EndMethod();
                }
            }
        }

        public void RemoveValue(string key)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");
                try
                {
                    secureStorageDependencyService.Remove(key);
                }
                catch (Exception ex)
                {
                    loggerService.Exception("Failed to remove from secure-storage.", ex);
                }
                finally
                {
                    loggerService.EndMethod();
                }
            }
        }

        public int GetIntValue(string key, int defaultValue = default)
        {
            return GetValue(key, defaultValue);
        }

        public long GetLongValue(string key, long defaultValue = default)
        {
            return GetValue(key, defaultValue);
        }

        public float GetFloatValue(string key, float defaultValue = default)
        {
            return GetValue(key, defaultValue);
        }

        public string GetStringValue(string key, string defaultValue = default)
        {
            return GetValue(key, defaultValue);
        }

        public bool GetBoolValue(string key, bool defaultValue = default)
        {
            return GetValue(key, defaultValue);
        }

        public double GetDoubleValue(string key, double defaultValue = default)
        {
            return GetValue(key, defaultValue);
        }

        public void SetIntValue(string key, int value)
        {
            SetValue(key, value);
        }

        public void SetLongValue(string key, long value)
        {
            SetValue(key, value);
        }

        public void SetFloatValue(string key, float value)
        {
            SetValue(key, value);
        }

        public void SetStringValue(string key, string value)
        {
            SetValue(key, value);
        }

        public void SetBoolValue(string key, bool value)
        {
            SetValue(key, value);
        }

        public void SetDoubleValue(string key, double value)
        {
            SetValue(key, value);
        }
    }
}
