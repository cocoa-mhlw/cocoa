using Microsoft.Azure.Cosmos;

namespace Covid19Radar.DataStore
{
    public interface ICosmos
    {
        Container User { get; }
        Container Notification { get; }
        Container TemporaryExposureKey { get; }
        Container Diagnosis { get; }
    }
}
