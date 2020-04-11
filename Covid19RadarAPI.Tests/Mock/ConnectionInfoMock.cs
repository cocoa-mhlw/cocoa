using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Tests.Mock
{
    public class ConnectionInfoMock : ConnectionInfo
    {
        public override string Id { get; set; }
        public override IPAddress RemoteIpAddress { get; set; }
        public override int RemotePort { get; set; }
        public override IPAddress LocalIpAddress { get; set; }
        public override int LocalPort { get; set; }
        public override X509Certificate2 ClientCertificate { get; set; }

        public override Task<X509Certificate2> GetClientCertificateAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ClientCertificate);
        }
    }
}
