using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{
    public class CustomVerificationService : IVerificationService
    {
        private readonly IConfiguration Config;
        private readonly ILogger<CustomVerificationService> Logger;
        private readonly X509Certificate2 Cert;
        private readonly string ApiSecret;
        private readonly string Url;
        private readonly HttpClientHandler Handler;
        private readonly HttpClient Client;
        private readonly string VerificationPayloadParameterName;

        public CustomVerificationService(IConfiguration config,
                                         ILogger<CustomVerificationService> logger)
        {
            Config = config;
            Logger = logger;
            var cert = config.VerificationPayloadPfx();
            Cert = new X509Certificate2(Convert.FromBase64String(cert));
            Handler = new HttpClientHandler();
            Handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            Handler.ClientCertificates.Add(Cert);
            ApiSecret = config.VerificationPayloadApiSecret();
            Url = config.VerificationPayloadUrl();
            Client = new HttpClient(Handler);
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiSecret);
            VerificationPayloadParameterName = config.VerificationPayloadParameterName();
        }

        public async Task<bool> Verification(string verificationPayload)
        {
            var payload = $@"
""{VerificationPayloadParameterName}"": ""{verificationPayload}"" 
";
            var content = new StringContent(payload);
            var response = await Client.PostAsync(Url, content);
            return response.IsSuccessStatusCode;
        }
    }
}
