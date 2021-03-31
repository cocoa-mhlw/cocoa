/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.DataStore;
using Microsoft.Azure.Cosmos;
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
        private readonly ICustomVerificationStatusRepository CustomVerification;
        private readonly ILogger<CustomVerificationService> Logger;
        private readonly X509Certificate2 Cert;
        private readonly string ApiSecret;
        private readonly string Url;
        private readonly HttpClientHandler Handler;
        private readonly HttpClient Client;
        private readonly string VerificationPayloadParameterName;

        public CustomVerificationService(IConfiguration config,
                                         ICustomVerificationStatusRepository customVerification,   
                                         ILogger<CustomVerificationService> logger)
        {
            Config = config;
            CustomVerification = customVerification;
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

        public async Task<int> VerificationAsync(string verificationPayload)
        {
            Logger.LogInformation($"start {nameof(VerificationAsync)}");
            var payload = $@"{{
""{VerificationPayloadParameterName}"": ""{verificationPayload}""
}}";
            var content = new StringContent(payload);
            var response = await Client.PostAsync(Url, content);
            if (!response.IsSuccessStatusCode) return 503;

            var responseBody = JsonConvert.DeserializeObject<ResponseCustomVerification>(await response.Content.ReadAsStringAsync());
            Logger.LogInformation($"result code of payloadAPI is {responseBody.Result}");
            return await GetResultStatus(responseBody.Result);
        }

        public async Task<int> GetResultStatus(string responseResult)
        {
            var results = await CustomVerification.GetAsync();
            return results.FirstOrDefault(_ => _.Result == responseResult)?.HttpStatusCode ?? 503;
        }

        public class ResponseCustomVerification
        {
            [JsonProperty("result")]
            public string Result { get; set; }
        }
    }
}
