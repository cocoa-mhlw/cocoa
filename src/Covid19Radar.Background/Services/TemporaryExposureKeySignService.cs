/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Background.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public class TemporaryExposureKeySignService : ITemporaryExposureKeySignService
    {
        public readonly ILogger<TemporaryExposureKeySignService> Logger;
        public readonly string VerificationKeyId;
        public readonly string VerificationKeyVersion;
        public readonly string VerificationKeySecret;
        public readonly ECDsa VerificationKey;
        public readonly X509SignatureGenerator VerificationKeyGenerator;

        public TemporaryExposureKeySignService(
            IConfiguration config,
            ILogger<TemporaryExposureKeySignService> logger)
        {
            Logger = logger;
            Logger.LogInformation($"{nameof(TemporaryExposureKeySignService)} constructor");
            VerificationKeyId = config.VerificationKeyId();
            VerificationKeyVersion = config.VerificationKeyVersion();
            VerificationKeySecret = config.VerificationKeySecret();

            var key = Convert.FromBase64String(VerificationKeySecret);
            VerificationKey = ECDsa.Create();
            int read;
            VerificationKey.ImportECPrivateKey(key, out read);
            VerificationKeyGenerator = System.Security.Cryptography.X509Certificates.X509SignatureGenerator.CreateForECDsa(VerificationKey);
        }


        public Task<byte[]> SignAsync(MemoryStream source)
        {
            Logger.LogInformation($"start {nameof(SignAsync)}");
            var pos = source.Position;
            source.Position = 0;
            var signatureCert = VerificationKeyGenerator.SignData(source.ToArray(), HashAlgorithmName.SHA256);
            source.Position = pos;
            return Task.FromResult(signatureCert);

        }

        public Task SetSignatureAsync(SignatureInfo info)
        {
            Logger.LogInformation($"start {nameof(SetSignatureAsync)}");
            info.VerificationKeyId = VerificationKeyId;
            info.VerificationKeyVersion = VerificationKeyVersion;
            return Task.CompletedTask;
        }

    }
}
