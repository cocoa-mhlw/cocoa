using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
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
            // option client certification
            if (!string.IsNullOrWhiteSpace(cert))
            {
                Cert = new X509Certificate2(Convert.FromBase64String(cert));
                Handler = new HttpClientHandler();
                Handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                Handler.ClientCertificates.Add(Cert);
            }
            Url = config.VerificationPayloadUrl();
            Client = new HttpClient(Handler);
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            VerificationPayloadParameterName = config.VerificationPayloadParameterName();
            // option api secret for api management
            ApiSecret = config.VerificationPayloadApiSecret();
            if (!string.IsNullOrWhiteSpace(ApiSecret)) {
                Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiSecret);
            }
        }

        public async Task<bool> Verification(string verificationPayload)
        {
            var payload = $@"
""{VerificationPayloadParameterName}"": ""{verificationPayload}"" 
";
            var content = new StringContent(payload);
            var response = await Client.PostAsync(Url, content);
            if (!response.IsSuccessStatusCode) return false;

            var responseBody = JsonConvert.DeserializeObject<ResponseMock>(await response.Content.ReadAsStringAsync());
            return responseBody.IsValid();
        }

        public class ResponseMock
        {
            [JsonProperty("result")]
            public string Result { get; set; }
            public bool IsValid()
            {
                if (string.IsNullOrWhiteSpace(Result)) return false;
                if (Result.All(_ => _ == Result.First())) return false;
                return true;
            }
        }
    }
}
