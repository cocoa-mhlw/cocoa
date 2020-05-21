using Covid19Radar.Api.Protobuf;

namespace Covid19Radar.Background.Services
{
    public interface ITemporaryExposureKeySignatureInfoService
    {
        SignatureInfo Create();
    }
}
