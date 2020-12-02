using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    public interface IApplicationPropertyService
    {
        bool ContainsKey(string key);
        object GetProperties(string key);
        Task SavePropertiesAsync(string key, object property);
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
    }
}
