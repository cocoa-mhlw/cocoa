using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public interface ISequenceRepository
    {
        Task<ulong> GetNextAsync(string key, ulong startNo, int increment = 1);
        Task<ulong> GetNextAsync(Common.PartitionKeyRotation.KeyInformation key, ulong startNo, int increment = 1);
    }
}
