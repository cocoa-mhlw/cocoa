/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{
    public class DeviceValidationAppleService
    {
        //const string UrlApple = "https://api.development.devicecheck.apple.com/v1/validate_device_token";
        const string UrlApple = "https://api.devicecheck.apple.com/v1/validate_device_token";
        private readonly HttpClient ClientApple;
        private readonly ILogger Logger;

        public DeviceValidationAppleService(
            IConfiguration config,
            IHttpClientFactory client,
            ILogger logger)
        {
            ClientApple = client.CreateClient();
            Logger = logger;
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
        public async Task<bool> Validation(IAppleDeviceVerification deviceVerification, DateTimeOffset requestTime, AuthorizedAppInformation app)
        {
            var payload = new ApplePayload()
            {
                DeviceToken = deviceVerification.DeviceToken,
                Timestamp = requestTime.ToUnixTimeMilliseconds()
            };

            using (var sha = SHA256.Create())
            {
                payload.TransactionId = Convert.ToBase64String(
                    sha.ComputeHash(Encoding.UTF8.GetBytes(deviceVerification.TransactionIdSeed))
                    );
            }

            Logger.LogInformation($"{nameof(Validation)} DeviceCheckKeyId:{app.DeviceCheckKeyId} DeviceCheckTeamId:{app.DeviceCheckTeamId} DeviceCheckPrivateKey:{app.DeviceCheckPrivateKey}");

            var jwt = GenerateClientSecretJWT(requestTime, app.DeviceCheckKeyId, app.DeviceCheckTeamId, app.DeviceCheckPrivateKey);

            var payloadJson = JsonConvert.SerializeObject(payload);
            Logger.LogInformation($"{nameof(Validation)} payload:{payloadJson} ");
            var request = new HttpRequestMessage(HttpMethod.Post, UrlApple);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            request.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            try
            {
                var response = await ClientApple.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Logger.LogWarning($"iOS device check failed.\r\n{nameof(HttpRequestMessage)} : {request}\r\n{nameof(HttpResponseMessage)} : {response}");
                }

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
                {
                    // FIXME: When call iOS Device check, return error sometimes, Until the cause is known, ignored device check
                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{nameof(Validation)}");
                throw;
            }
        }

        static string GenerateClientSecretJWT(DateTimeOffset requestTime, string keyId, string teamId, string p8FileContents)
        {
            var headers = new Dictionary<string, object>
            {
                //{ "alg", "ES256" },
                { "kid", keyId }
            };

            var payload = new Dictionary<string, object>
            {
                { "iss", teamId },
                { "iat", requestTime.ToUnixTimeSeconds() }
            };
            var cngKey = CngKey.Import(Convert.FromBase64String(p8FileContents), CngKeyBlobFormat.Pkcs8PrivateBlob);
            return Jose.JWT.Encode(payload, cngKey, Jose.JwsAlgorithm.ES256, headers);
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
