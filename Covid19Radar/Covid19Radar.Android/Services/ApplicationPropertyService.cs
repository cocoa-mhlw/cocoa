/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Xamarin.Forms.Internals;

namespace Covid19Radar.Droid.Services
{
    public class ApplicationPropertyService : IApplicationPropertyService
    {
        private readonly ILoggerService loggerService;

        public ApplicationPropertyService(ILoggerService loggerService)
        {
            this.loggerService = loggerService;
        }

        public bool ContainsKey(string key)
        {
            return Properties.ContainsKey(key);
        }

        public object GetProperties(string key)
        {
            return Properties[key];
        }

        public async Task SavePropertiesAsync(string key, object property)
        {
            Properties[key] = property;
            await SavePropertiesAsync();
        }

        public async Task Remove(string key)
        {
            Properties.Remove(key);
            await SavePropertiesAsync();
        }

        private Xamarin.Forms.Application CurrentApplication
        {
            get
            {
                return Xamarin.Forms.Application.Current;
            }
        }

        private Task<IDictionary<string, object>> _propertiesTask = null;
        private IDictionary<string, object> Properties
        {
            get
            {
                if (CurrentApplication?.Properties != null)
                {
                    loggerService.Info("Use Application.Current.Properties");
                    return CurrentApplication.Properties;
                }
                if (_propertiesTask == null)
                {
                    _propertiesTask = GetPropertiesAsync();
                    loggerService.Info("Use self-created Properties");
                }
                return _propertiesTask.Result;
            }
        }

        private IDeserializer _deserializer = null;
        private IDeserializer Deserializer
        {
            get
            {
                if (_deserializer == null)
                {
                    _deserializer = CreateDeserializer();
                }
                return _deserializer;
            }
        }

        private readonly SemaphoreSlim _semaphoreForDeserializer = new SemaphoreSlim(1, 1);

        private async Task SavePropertiesAsync()
        {
            _semaphoreForDeserializer.Wait();
            try
            {
                if (CurrentApplication != null)
                {
                    await CurrentApplication.SavePropertiesAsync();
                    loggerService.Info("Saved using Application.Current.Properties");
                }
                else
                {
                    await Deserializer.SerializePropertiesAsync(Properties);
                    loggerService.Info("Saved using self-created serializer");
                }
            }
            finally
            {
                _semaphoreForDeserializer.Release();
            }
        }

        private async Task<IDictionary<string, object>> GetPropertiesAsync()
        {
            _semaphoreForDeserializer.Wait();
            try
            {
                IDictionary<string, object> properties = await Deserializer.DeserializePropertiesAsync().ConfigureAwait(false);
                if (properties == null)
                    properties = new Dictionary<string, object>(4);
                return properties;
            }
            finally
            {
                _semaphoreForDeserializer.Release();
            }
        }

        private IDeserializer CreateDeserializer()
        {
            // Use reflection to get the internal class
            var asm = typeof(Xamarin.Forms.Platform.Android.Platform).Assembly;
            var type = asm.GetType("Xamarin.Forms.Platform.Android.Deserializer");
            var deserializer = (IDeserializer)Activator.CreateInstance(type);
            return deserializer;
        }
    }
}
