namespace Covid19Radar.Services
{
    public interface IEssentialsService
    {
        // DeviceInfo
        string Platform { get; }
        string PlatformVersion { get; }
        string Model { get; }
        string DeviceType { get; }

        // AppInfo
        string AppVersion { get; }
        string BuildNumber { get; }
    }
}
