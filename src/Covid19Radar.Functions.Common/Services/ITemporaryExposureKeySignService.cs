using Covid19Radar.Protobuf;
using System.IO;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface ITemporaryExposureKeySignService
    {

        Task<byte[]> SignAsync(Stream source);

        Task SetSignatureAsync(SignatureInfo info);
    }

}
