using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Covid19Radar.Api.Models;

namespace Covid19Radar.Api.Extensions
{
    public static class IAndroidDeviceVerificationExtensions
    {
        public static byte[] GetAndroidNonce(this IAndroidDeviceVerification submission)
        {
            var cleartext = GetAndroidNonceClearText(submission);
            var nonce = GetSha256(cleartext);
            return nonce;
        }

        private static string GetAndroidNonceClearText(this IAndroidDeviceVerification submission)
                => string.Join("|", submission.AppPackageName, submission.KeyString, GetRegionString(submission.Regions), submission.VerificationPayload);

        private static string GetRegionString(string[] regions)
            => string.Join(",", regions.Select(r => r.ToUpperInvariant()).OrderBy(r => r));

        private static byte[] GetSha256(string text)
        {
            using var sha = SHA256.Create();
            var textBytes = Encoding.UTF8.GetBytes(text);
            return sha.ComputeHash(textBytes);
        }
    }
}
