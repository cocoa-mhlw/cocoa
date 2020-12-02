using Xamarin.Essentials;

namespace Covid19Radar.Services
{
    public class EssentialsService : IEssentialsService
    {
        // DeviceInfo
        public string Platform => DeviceInfo.Platform.ToString();
        public string PlatformVersion => DeviceInfo.VersionString;
        public string Model => DeviceInfo.Model;
        public string DeviceType => DeviceInfo.DeviceType.ToString();

        // AppInfo
        public string AppVersion => AppInfo.VersionString;
        public string BuildNumber => AppInfo.BuildString;
    }
}
