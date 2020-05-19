using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface ITemporaryExposureKeySignService
    {

        Task<byte[]> SignAsync(Stream source);

        Task<byte[]> GetPublicKeyAsync();
    }

    public static class ITemporaryExposureKeySignServiceExtension
    {
        public static async Task<X509Certificate2> GetX509PublicKeyAsync(this ITemporaryExposureKeySignService service)
        {
            var p = await service.GetPublicKeyAsync();
            return new X509Certificate2(p);
        }
    }
}
