using Covid19Radar.Api.Models;
using Covid19Radar.Background.Protobuf;
using Google.Protobuf;

namespace Covid19Radar.Background.Extentions
{
    public static class TemporaryExposureKeyModelExtension
    {
        public static TemporaryExposureKey ToKey(this TemporaryExposureKeyModel tek)
        {
            return new TemporaryExposureKey()
            {
                KeyData = ByteString.CopyFrom(tek.KeyData),
                RollingStartIntervalNumber = tek.RollingStartIntervalNumber,
                RollingPeriod = tek.RollingPeriod,
                TransmissionRiskLevel = tek.TransmissionRiskLevel
            };
        }
    }
}
