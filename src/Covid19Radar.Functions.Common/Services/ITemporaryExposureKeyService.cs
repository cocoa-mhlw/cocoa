using Covid19Radar.Models;
using Covid19Radar.Protobuf;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface ITemporaryExposureKeyService
    {
        Task RunAsync();
    }

}
