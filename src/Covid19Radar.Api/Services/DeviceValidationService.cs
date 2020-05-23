using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Covid19Radar.Api.Models;
using System.Linq;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using System.Net;
using System.Text.Json.Serialization;

namespace Covid19Radar.Api.Services
{
    public class DeviceValidationService : IDeviceValidationService
    {
        const string UrlAndroid = "https://www.googleapis.com/androidcheck/v1/attestations/verify?key=";
        private readonly HttpClient ClientAndroid;
        private readonly string AndroidBearerToken;

        const string UrlApple = "https://api.development.devicecheck.apple.com/v1/validate_device_token";
        private readonly HttpClient ClientApple;
        private readonly string AppleBearerToken;

        public DeviceValidationService(
            IConfiguration config,
            IHttpClientFactory client)
        {
            AndroidBearerToken = config["AndroidBearerToken"];
            ClientAndroid = client.CreateClient();
            AppleBearerToken = config["AppleBearerToken"];
            ClientApple = client.CreateClient();
        }


        public async Task<bool> Validation(DiagnosisSubmissionParameter param)
        {
            switch(param.Platform) {
                case "android":
                    return await ValidationAndroid(param);
                case "ios":
                    return await ValidationApple(param);
            }
            return false;
        }

        /// <summary>
        /// for Apple
        /// </summary>
        public class ApplePayload
        {
            [JsonProperty("device_token")]
            public string DeviceToken { get; set; }
            [JsonProperty("transaction_id")]
            public string TransactionId { get; set; }
            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }
        }

        /// <summary>
        /// Validation iOS
        /// </summary>
        /// <param name="param">subumission parameter</param>
        /// <returns>True when successful.</returns>
        /// <remarks>
        /// https://developer.apple.com/documentation/devicecheck/accessing_and_modifying_per-device_data
        /// </remarks>
        public async Task<bool> ValidationApple(DiagnosisSubmissionParameter param)
        {
            var payload = new ApplePayload()
            {
                DeviceToken = param.DeviceVerificationPayload,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            var keysText = param.Keys
                .OrderBy(_ => _.KeyData)
                .Select(_ => _.KeyData)
                .Aggregate((a, b) => a + b);

            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var value = System.Text.Encoding.UTF8.GetBytes(param.AppPackageName + keysText + param.Region);
                payload.TransactionId = Convert.ToBase64String(sha.ComputeHash(value));
            }

            var content = new StringContent(JsonConvert.SerializeObject(payload));
            var request = new HttpRequestMessage(HttpMethod.Post, UrlApple);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppleBearerToken);
            var response = await ClientApple.SendAsync(request);

            switch (response.StatusCode)
            {
                // 200 OK:                  The transaction was successful
                // 200 Bit State Not Found: The bit state wasn't found
                case System.Net.HttpStatusCode.OK:
                    if (response.ReasonPhrase == "OK") return true;

                    break;
                // 
                default:
                    break;
            }

            return false;
        }


        /// <summary>
        /// for Android
        /// </summary>
        public class AndroidPayload
        {
            [JsonProperty("signedAttestation")]
            public string SignedAttestation { get; set; }
        }
        public class AndroidResponse
        {
            [JsonProperty("isValidSignature")]
            public bool IsValidSignature { get; set; }
        }

        /// <summary>
        /// Validation Android
        /// </summary>
        /// <param name="param">subumission parameter</param>
        /// <returns>True when successful.</returns>
        public async Task<bool> ValidationAndroid(DiagnosisSubmissionParameter param)
        {
            try
            {
                var token = new JwtSecurityToken(param.DeviceVerificationPayload);

                // request
                var payload = new AndroidPayload()
                {
                    SignedAttestation = param.DeviceVerificationPayload
                };
                var content = new StringContent(JsonConvert.SerializeObject(payload));
                content.Headers.ContentType.MediaType = "application/json";
                var request = new HttpRequestMessage(HttpMethod.Post, UrlAndroid + AndroidBearerToken);

                // response
                var response = await ClientApple.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<AndroidResponse>(responseBody);
                return result.IsValidSignature;
            }
            catch (Exception) { }
            return false;
        }
    }
}
