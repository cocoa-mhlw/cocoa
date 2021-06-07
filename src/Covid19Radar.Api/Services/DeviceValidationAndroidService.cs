/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{
    public class DeviceValidationAndroidService
    {

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
        public  bool Validation(DiagnosisSubmissionParameter param, byte[] expectedNonce, DateTimeOffset requestTime, AuthorizedAppInformation app)
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

        private AndroidAttestationStatement ParsePayload(string signedAttestationStatement)
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
