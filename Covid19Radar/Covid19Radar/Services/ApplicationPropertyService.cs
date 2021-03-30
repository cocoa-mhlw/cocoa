/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    public interface IApplicationPropertyService
    {
        bool ContainsKey(string key);
        object GetProperties(string key);
        Task SavePropertiesAsync(string key, object property);
        Task Remove(string key);
    }

    public class ApplicationPropertyService : IApplicationPropertyService
    {
        public bool ContainsKey(string key)
        {
            return Application.Current.Properties.ContainsKey(key);
        }

        public object GetProperties(string key)
        {
            return Application.Current.Properties[key];
        }

        public async Task SavePropertiesAsync(string key, object property)
        {
            Application.Current.Properties[key] = property;
            await Application.Current.SavePropertiesAsync();
        }

        public async Task Remove(string key)
        {
            Application.Current.Properties.Remove(key);
            await Application.Current.SavePropertiesAsync();
        }
    }
}
