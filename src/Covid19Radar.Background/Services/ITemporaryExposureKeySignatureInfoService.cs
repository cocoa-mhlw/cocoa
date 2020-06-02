using Covid19Radar.Background.Protobuf;

namespace Covid19Radar.Background.Services
{
    public interface ITemporaryExposureKeySignatureInfoService
    {
        SignatureInfo Create();
    }
}
