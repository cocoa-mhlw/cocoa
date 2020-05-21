using Microsoft.Azure.Cosmos;

namespace Covid19Radar.Background.DataStore
{
    public interface IStoringCosmos
    {
        Container Contact { get; }
    }
}
