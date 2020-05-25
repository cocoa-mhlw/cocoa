using Covid19Radar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Covid19Radar.Model
{
	public static class AndroidExtensions
	{
		public static byte[] GetAndroidNonce(this SelfDiagnosisSubmission submission)
		{
			var cleartext = GetAndroidNonceClearText(submission);
			var nonce = GetSha256(cleartext);
			return nonce;
		}

		static string GetAndroidNonceClearText(this SelfDiagnosisSubmission submission) =>
			string.Join("|", submission.AppPackageName, GetKeyString(submission.Keys), GetRegionString(submission.Regions), submission.VerificationPayload);

		static string GetKeyString(IEnumerable<ExposureKey> keys) =>
			string.Join(",", keys.OrderBy(k => k.Key).Select(k => GetKeyString(k)));

		static string GetKeyString(ExposureKey k) =>
			string.Join(".", k.Key, k.RollingStart, k.RollingDuration, k.TransmissionRisk);

		static string GetRegionString(IEnumerable<string> regions) =>
			string.Join(",", regions.Select(r => r.ToUpperInvariant()).OrderBy(r => r));

		static byte[] GetSha256(string text)
		{
			using var sha = SHA256.Create();
			var textBytes = Encoding.UTF8.GetBytes(text);
			return sha.ComputeHash(textBytes);
		}
	}
}
