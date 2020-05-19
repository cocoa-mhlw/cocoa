using Covid19Radar.Protobuf;
using Microsoft.Azure.KeyVault.WebKey;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface ITemporaryExposureKeySignService
    {

        Task<byte[]> SignAsync(Stream source);

        Task SetSignatureAsync(SignatureInfo info);
    }

}
