using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Covid19Radar.Services
{

#nullable enable
    public interface IApplicationPropertyService
    {
        Task<bool> ContainsKeyAsync(string key);
        Task<object> GetPropertiesAsync(string key);
        Task SavePropertiesAsync(string key, object property);
        Task RemoveAsync(string key);
    }

    public class ApplicationPropertyService : IApplicationPropertyService
    {
        private readonly IDeserializer deserializer;
        private IDictionary<string, object>? properties = null;

        public ApplicationPropertyService(IDeserializer deserializer)
        {
            this.deserializer = deserializer;
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            properties = await LoadPropertiesAsync();
            return properties.ContainsKey(key);
        }

        public async Task<object> GetPropertiesAsync(string key)
        {
            properties = await LoadPropertiesAsync();
            return properties[key];
        }
        　
        public Task SavePropertiesAsync(string key, object property)
        {
            if (properties == null)
            {
                return Task.CompletedTask;
            }
            properties[key] = property;
            return deserializer.SerializePropertiesAsync(properties);
        }

        public Task RemoveAsync(string key)
        {
            if (properties == null)
            {
                return Task.CompletedTask;
            }
            properties.Remove(key);
            return deserializer.SerializePropertiesAsync(properties);
        }

        private async Task<IDictionary<string, object>> LoadPropertiesAsync()
        {
            if (properties != null)
            {
                return properties;
            }

            var prop = await deserializer.DeserializePropertiesAsync();
            properties = prop != null ? prop : new Dictionary<string, object>();
            return properties;
        }

    }
}
