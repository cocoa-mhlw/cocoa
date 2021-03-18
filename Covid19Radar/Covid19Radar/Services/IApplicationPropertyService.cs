using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface IApplicationPropertyService
    {
        bool ContainsKey(string key);
        object GetProperties(string key);
        Task SavePropertiesAsync(string key, object property);
        Task RemoveAsync(string key);
    }
}
