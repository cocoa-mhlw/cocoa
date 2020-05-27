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

        private readonly IAuthorizedAppRepository AuthApp;

        public DeviceValidationService(
            IConfiguration config,
            IAuthorizedAppRepository authApp,
            IHttpClientFactory client)
        {
            AndroidBearerToken = config.AndroidBearerToken();
            ClientAndroid = client.CreateClient();
            AppleBearerToken = config.AppleBearerToken();
            ClientApple = client.CreateClient();
            AuthApp = authApp;
        }

        public async Task<bool> Validation(DiagnosisSubmissionParameter param, DateTimeOffset requestTime)
        {
            var app = await AuthApp.GetAsync(param.Platform);
            switch (param.Platform)
            {
                case "android":
                    return await ValidationAndroid(param, param.GetAndroidNonce(), requestTime, app);
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
                var value = System.Text.Encoding.UTF8.GetBytes(param.AppPackageName + keysText + string.Join(',', param.Regions));
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
        /// <param name="param">subumission parameter</param>
        /// <returns>True when successful.</returns>
        public async Task<bool> ValidationAndroid(DiagnosisSubmissionParameter param, byte[] expectedNonce, DateTimeOffset requestTime, AuthorizedApp app)
        {
            var claims = ParsePayload(param.DeviceVerificationPayload);

            // Validate the nonce
            if (Convert.ToBase64String(claims.Nonce) != Convert.ToBase64String(expectedNonce))
                return false;

            // Validate time interval
            var now = requestTime.ToUnixTimeMilliseconds();
            if (app.SafetyNetPastTimeSeconds > 0)
            {
                var minTime = now - (app.SafetyNetPastTimeSeconds * 1000);
                if (claims.TimestampMilliseconds < minTime)
                    return false;
            }
            if (app.SafetyNetFutureTimeSeconds > 0)
            {
                var minTime = now + (app.SafetyNetFutureTimeSeconds * 1000);
                if (claims.TimestampMilliseconds > minTime)
                    return false;
            }

            // Validate certificate
            if (app.SafetyNetApkDigestSha256?.Length > 0)
            {
                var apkSha = Convert.ToBase64String(claims.ApkCertificateDigestSha256);
                if (!app.SafetyNetApkDigestSha256.Contains(apkSha))
                    return false;
            }

            // Validate integrity
            if (app.SafetyNetCtsProfileMatch)
            {
                if (!claims.CtsProfileMatch)
                    return false;
            }
            if (app.SafetyNetBasicIntegrity)
            {
                if (!claims.BasicIntegrity)
                    return false;
            }

            return true;
        }

        public static AndroidAttestationStatement ParsePayload(string signedAttestationStatement)
        {
            // First parse the token and get the embedded keys.
            JwtSecurityToken token;
            try
            {
                token = new JwtSecurityToken(signedAttestationStatement);
            }
            catch (ArgumentException)
            {
                // The token is not in a valid JWS format.
                return null;
            }

            // We just want to validate the authenticity of the certificate.
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = GetEmbeddedKeys(token)
            };

            // Perform the validation
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            try
            {
                tokenHandler.ValidateToken(signedAttestationStatement, validationParameters, out validatedToken);
            }
            catch (ArgumentException)
            {
                // Signature validation failed.
                return null;
            }

            // Verify the hostname
            if (!(validatedToken.SigningKey is X509SecurityKey))
                return null;

            if (GetHostName(validatedToken.SigningKey as X509SecurityKey) != "attest.android.com")
                return null;

            // Parse and use the data JSON.
            var claimsDictionary = token.Claims.ToDictionary(x => x.Type, x => x.Value);
            return new AndroidAttestationStatement(claimsDictionary);
        }

        static string GetHostName(X509SecurityKey securityKey)
        {
            try
            {
#if DEBUG
                using var chain = new X509Chain();
                var chainBuilt = chain.Build(securityKey.Certificate);
                if (!chainBuilt)
                {
                    var s = string.Empty;
                    foreach (var chainStatus in chain.ChainStatus)
                    {
                        s += $"Chain error: {chainStatus.Status} {chainStatus.StatusInformation}\n";
                    }
                }
#endif

                if (!securityKey.Certificate.Verify())
                    return null;

                return securityKey.Certificate.GetNameInfo(X509NameType.DnsName, false);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        static X509SecurityKey[] GetEmbeddedKeys(JwtSecurityToken token) =>
            (token.Header["x5c"] as IEnumerable)
            .Cast<object>()
            .Select(x => x.ToString())
            .Select(x => new X509SecurityKey(new X509Certificate2(Convert.FromBase64String(x))))
            .ToArray();

        public sealed class AndroidAttestationStatement
        {
            public AndroidAttestationStatement(Dictionary<string, string> claims)
            {
                Claims = claims;

                if (claims.ContainsKey("nonce"))
                    Nonce = Convert.FromBase64String(claims["nonce"]);

                if (claims.ContainsKey("timestampMs") && long.TryParse(claims["timestampMs"], NumberStyles.Integer, CultureInfo.InvariantCulture, out var timestampMsLocal))
                    TimestampMilliseconds = timestampMsLocal;

                if (claims.ContainsKey("apkPackageName"))
                    ApkPackageName = claims["apkPackageName"];

                if (claims.ContainsKey("apkDigestSha256"))
                    ApkDigestSha256 = Convert.FromBase64String(claims["apkDigestSha256"]);

                if (claims.ContainsKey("apkCertificateDigestSha256"))
                    ApkCertificateDigestSha256 = Convert.FromBase64String(claims["apkCertificateDigestSha256"]);

                if (claims.ContainsKey("ctsProfileMatch") && bool.TryParse(claims["ctsProfileMatch"], out var ctsProfileMatchLocal))
                    CtsProfileMatch = ctsProfileMatchLocal;

                if (claims.ContainsKey("basicIntegrity") && bool.TryParse(claims["basicIntegrity"], out var basicIntegrityLocal))
                    BasicIntegrity = basicIntegrityLocal;
            }

            public IReadOnlyDictionary<string, string> Claims { get; }

            public byte[] Nonce { get; }

            public long TimestampMilliseconds { get; }

            public string ApkPackageName { get; }

            public byte[] ApkDigestSha256 { get; }

            public byte[] ApkCertificateDigestSha256 { get; }

            public bool CtsProfileMatch { get; }

            public bool BasicIntegrity { get; }
        }
    }
}
