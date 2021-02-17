using Covid19Radar.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Covid19Radar.Api.Extensions
{
    public static class V1AndroidExtensions
	{
		public static byte[] GetAndroidNonce(this V1DiagnosisSubmissionParameter submission)
		{
			var cleartext = GetAndroidNonceClearText(submission);
			var nonce = GetSha256(cleartext);
			return nonce;
		}

		static string GetAndroidNonceClearText(this V1DiagnosisSubmissionParameter submission) =>
			string.Join("|", submission.AppPackageName, GetKeyString(submission.Keys), GetRegionString(submission.Regions), submission.VerificationPayload);

		static string GetKeyString(IEnumerable<V1DiagnosisSubmissionParameter.Key> keys) =>
			string.Join(",", keys.OrderBy(k => k.KeyData).Select(k => GetKeyString(k)));

		 static string GetKeyString(V1DiagnosisSubmissionParameter.Key k) =>
			string.Join(".", k.KeyData, k.RollingStartNumber, k.RollingPeriod, k.TransmissionRisk);

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
