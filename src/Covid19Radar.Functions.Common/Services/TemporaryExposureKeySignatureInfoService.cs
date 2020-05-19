using Covid19Radar.Protobuf;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Covid19Radar.Services
{
    public class TemporaryExposureKeySignatureInfoService : ITemporaryExposureKeySignatureInfoService
    {
        public readonly string AppBundleId;
        public readonly string AndroidPackage;
        public readonly string Region;
        public readonly string Algorithm = "1.2.840.10045.4.3.2";
        public readonly SignatureInfo Info;

        public TemporaryExposureKeySignatureInfoService(IConfiguration config)
        {
            AppBundleId = config["AppBundleId"];
            AndroidPackage = config["AndroidPackage"];
            Region = config["Region"];
            Info = new SignatureInfo();
            Info.AppBundleId = AppBundleId;
            Info.AndroidPackage = AndroidPackage;
            Info.SignatureAlgorithm = Algorithm;
        }

        public SignatureInfo Create(X509Certificate2 key)
        {
            var si = new SignatureInfo(Info);
            // TODO: Value to set
            si.VerificationKeyId = key.Subject;
            si.VerificationKeyVersion = key.Version.ToString();
            return si;
        }

    }
}
