/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Covid19Radar.Services;

namespace Covid19Radar.iOS.Services
{
    public class ApplicationPropertyService : IApplicationPropertyService
    {
        public bool ContainsKey(string key)
        {
            return Xamarin.Forms.Application.Current.Properties.ContainsKey(key);
        }

        public object GetProperties(string key)
        {
            return Xamarin.Forms.Application.Current.Properties[key];
        }

        public async Task SavePropertiesAsync(string key, object property)
        {
            Xamarin.Forms.Application.Current.Properties[key] = property;
            await Xamarin.Forms.Application.Current.SavePropertiesAsync();
        }

        public async Task Remove(string key)
        {
            Xamarin.Forms.Application.Current.Properties.Remove(key);
            await Xamarin.Forms.Application.Current.SavePropertiesAsync();
        }
    }
}
