using Microsoft.Azure.Cosmos;

namespace Covid19Radar.DataStore
{
    public interface IStoringCosmos
    {
        Container Contact { get; }
    }
}
