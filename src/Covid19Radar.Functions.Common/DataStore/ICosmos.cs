using Microsoft.Azure.Cosmos;

namespace Covid19Radar.DataStore
{
    public interface ICosmos
    {
        Container User { get; }
        Container Beacon { get; }
        Container Sequence { get; }
        Container Otp { get; }
        Container Notification { get; }
        Container BeaconUuid { get; }
        Container Infection { get; }
        Container InfectionProcess { get; }
        Container TemporaryExposureKey { get; }
        string ContainerNameBeacon { get; }
    }
}
