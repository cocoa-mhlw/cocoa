using Covid19Radar.Models;
using Covid19Radar.Protobuf;
using System.Collections.Generic;

namespace Covid19Radar.Services
{
    public interface ITemporaryExposureKeyService
    {
        TEKSignature CreateSignature(int batchNum, int batchSize);
        IEnumerator<TemporaryExposureKeyExportModel> Create(ulong startTimestamp, ulong endTimestamp, IEnumerable<TemporaryExposureKey> keys);
    }

}
