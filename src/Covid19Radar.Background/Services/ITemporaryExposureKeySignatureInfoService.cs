using Covid19Radar.Protobuf;

namespace Covid19Radar.Background.Services
{
    public interface ITemporaryExposureKeySignatureInfoService
    {
        SignatureInfo Create();
    }
}
