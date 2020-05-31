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
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using Covid19Radar.Api.DataAccess;
using System.Security.Cryptography;
using Covid19Radar.Api.Extensions;
using System.Text.RegularExpressions;

namespace Covid19Radar.Api.Services
{
    public class DeviceValidationAppleService
    {
        const string UrlApple = "https://api.development.devicecheck.apple.com/v1/validate_device_token";
        private readonly HttpClient ClientApple;
        private readonly string AppleBearerToken;

        public DeviceValidationAppleService(
            IConfiguration config,
            IHttpClientFactory client)
        {
            AppleBearerToken = config.AppleBearerToken();
            ClientApple = client.CreateClient();
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
        public async Task<bool> Validation(DiagnosisSubmissionParameter param, DateTimeOffset requestTime, AuthorizedAppInformation app)
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
                var value = System.Text.Encoding.UTF8.GetBytes(param.AppPackageName + keysText + string.Join(',', param.Regions));
                payload.TransactionId = Convert.ToBase64String(sha.ComputeHash(value));
            }

            var jwt = GenerateClientSecretJWT(requestTime, app.DeviceCheckKeyId, app.DeviceCheckTeamId, app.DeviceCheckPrivateKey);

            var content = new StringContent(JsonConvert.SerializeObject(payload));
            var request = new HttpRequestMessage(HttpMethod.Post, UrlApple);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var response = await ClientApple.SendAsync(request);

            //switch (response.StatusCode)
            //{
            //    // 200 OK:                  The transaction was successful
            //    // 200 Bit State Not Found: The bit state wasn't found
            //    case System.Net.HttpStatusCode.OK:
            //        if (response.ReasonPhrase == "OK") return true;

            //        break;
            //    // 
            //    default:
            //        break;
            //}
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return false;

            return true;
        }

        static string GenerateClientSecretJWT(DateTimeOffset requestTime, string keyId, string teamId, string p8FileContents)
        {
            var headers = new Dictionary<string, object>
            {
                { "alg", "ES256" },
                { "kid", keyId }
            };

            var payload = new Dictionary<string, object>
            {
                { "iss", teamId },
                { "iat", requestTime.ToUnixTimeMilliseconds() }
            };

            var secretKey = CleanP8Key(p8FileContents);

            // Get our headers/payloads into a json string
            var headerStr = "{" + string.Join(",", headers.Select(kvp => $"\"{kvp.Key}\":\"{kvp.Value.ToString()}\"")) + "}";
            var payloadStr = "{";
            foreach (var kvp in payload)
            {
                if (kvp.Value is int || kvp.Value is long || kvp.Value is double)
                    payloadStr += $"\"{kvp.Key}\":{kvp.Value.ToString()},";
                else
                    payloadStr += $"\"{kvp.Key}\":\"{kvp.Value.ToString()}\",";
            }
            payloadStr = payloadStr.TrimEnd(',') + "}";


            // Load the key text
            var key = CngKey.Import(Convert.FromBase64String(secretKey), CngKeyBlobFormat.Pkcs8PrivateBlob);

            using var dsa = new ECDsaCng(key);
            var unsignedJwt = Base64UrlEncode(Encoding.UTF8.GetBytes(headerStr))
                                    + "." + Base64UrlEncode(Encoding.UTF8.GetBytes(payloadStr));

            var signature = dsa.SignData(Encoding.UTF8.GetBytes(unsignedJwt), HashAlgorithmName.SHA256);

            return unsignedJwt + "." + Base64UrlEncode(signature);
        }

        static string CleanP8Key(string p8Contents)
        {
            // Remove whitespace
            var tmp = Regex.Replace(p8Contents, "\\s+", string.Empty, RegexOptions.Singleline);

            // Remove `---- BEGIN PRIVATE KEY ----` bits
            tmp = Regex.Replace(tmp, "-{1,}.*?-{1,}", string.Empty, RegexOptions.Singleline);

            return tmp;
        }

        static string Base64UrlEncode(byte[] data)
        {
            var base64 = Convert.ToBase64String(data, 0, data.Length);
            var base64Url = new StringBuilder();

            foreach (var c in base64)
            {
                if (c == '+')
                    base64Url.Append('-');
                else if (c == '/')
                    base64Url.Append('_');
                else if (c == '=')
                    break;
                else
                    base64Url.Append(c);
            }

            return base64Url.ToString();
        }

    }
}
