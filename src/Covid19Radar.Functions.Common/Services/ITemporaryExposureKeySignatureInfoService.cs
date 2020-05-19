using Covid19Radar.Protobuf;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Covid19Radar.Services
{
    public interface ITemporaryExposureKeySignatureInfoService
    {
        SignatureInfo Create();
    }
}
