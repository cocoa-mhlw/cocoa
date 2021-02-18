namespace Covid19Radar.Services
{
    public interface IPreferencesService
    {
        T GetValue<T>(string key, T defaultValue);
        void SetValue<T>(string key, T value);
        void RemoveValue(string key);
        bool ContainsKey(string key);
    }
}
