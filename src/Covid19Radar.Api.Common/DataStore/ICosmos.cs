using Microsoft.Azure.Cosmos;

namespace Covid19Radar.Api.DataStore
{
    public interface ICosmos
    {
        Container User { get; }
        Container Notification { get; }
        Container TemporaryExposureKey { get; }
        Container Diagnosis { get; }
        Container TemporaryExposureKeyExport { get; }
        Container Sequence { get; }
    }
}
