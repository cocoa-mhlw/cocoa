using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExposureNotification.Backend.DeviceVerification
{
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
