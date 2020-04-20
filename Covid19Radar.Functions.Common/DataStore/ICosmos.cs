using Microsoft.Azure.Cosmos;

namespace Covid19Radar.DataStore
{
    public interface ICosmos
    {
        Container User { get; }
        Container Beacon { get; }
        Container Sequence { get; }
        Container Otp { get; }
    }
}
