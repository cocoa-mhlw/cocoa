using Covid19Radar.Api.Protobuf;
using System.IO;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public interface ITemporaryExposureKeySignService
    {

        Task<byte[]> SignAsync(Stream source);

        Task SetSignatureAsync(SignatureInfo info);
    }

}
