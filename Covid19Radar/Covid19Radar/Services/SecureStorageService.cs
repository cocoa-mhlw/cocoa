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
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");
                try
                {
                    if (secureStorageDependencyService.ContainsKey(key))
                    {
                        var bytes = secureStorageDependencyService.GetBytes(key);
                        return BitConverter.ToInt32(bytes);
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

            return defaultValue;
        }

        public long GetLongValue(string key, long defaultValue = default)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");
                try
                {
                    if (secureStorageDependencyService.ContainsKey(key))
                    {
                        var bytes = secureStorageDependencyService.GetBytes(key);
                        return BitConverter.ToInt64(bytes);
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

            return defaultValue;
        }

        public float GetFloatValue(string key, float defaultValue = default)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");
                try
                {
                    if (secureStorageDependencyService.ContainsKey(key))
                    {
                        var bytes = secureStorageDependencyService.GetBytes(key);
                        return BitConverter.ToSingle(bytes);
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

            return defaultValue;
        }

        public string GetStringValue(string key, string defaultValue = default)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");
                try
                {
                    if (secureStorageDependencyService.ContainsKey(key))
                    {
                        var bytes = secureStorageDependencyService.GetBytes(key);
                        return System.Text.Encoding.UTF8.GetString(bytes);
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

            return defaultValue;
        }

        public bool GetBoolValue(string key, bool defaultValue = default)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");
                try
                {
                    if (secureStorageDependencyService.ContainsKey(key))
                    {
                        var bytes = secureStorageDependencyService.GetBytes(key);
                        return BitConverter.ToBoolean(bytes);
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

            return defaultValue;
        }

        public double GetDoubleValue(string key, double defaultValue = default)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");
                try
                {
                    if (secureStorageDependencyService.ContainsKey(key))
                    {
                        var bytes = secureStorageDependencyService.GetBytes(key);
                        return BitConverter.ToDouble(bytes);
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

            return defaultValue;
        }

        public void SetIntValue(string key, int value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                try
                {
                    byte[] bytes = BitConverter.GetBytes(value);
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

        public void SetLongValue(string key, long value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                try
                {
                    byte[] bytes = BitConverter.GetBytes(value);
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

        public void SetFloatValue(string key, float value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                try
                {
                    byte[] bytes = BitConverter.GetBytes(value);
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

        public void SetStringValue(string key, string value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                try
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
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

        public void SetBoolValue(string key, bool value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                try
                {
                    byte[] bytes = BitConverter.GetBytes(value);
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

        public void SetDoubleValue(string key, double value)
        {
            lock (this)
            {
                loggerService.StartMethod();
                loggerService.Info($"key={key}");

                try
                {
                    byte[] bytes = BitConverter.GetBytes(value);
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
    }
}
