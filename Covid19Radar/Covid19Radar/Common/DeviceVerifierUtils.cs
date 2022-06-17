﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Covid19Radar.Model;

namespace Covid19Radar.Common
{
    public static class DeviceVerifierUtils
    {
        #region V3DiagnosisApi v2.0.0 -
        public static byte[] CreateAndroidNonceV3(DiagnosisSubmissionParameter submission)
        {
            var cleartext = GetNonceClearTextV3(submission);
            var nonce = GetSha256(cleartext);
            return nonce;
        }

        public static byte[] CreateAndroidNonceV3(V1EventLogRequest eventLogRequest)
        {
            var cleartext = GetNonceClearTextV3(eventLogRequest);
            var nonce = GetSha256(cleartext);
            return nonce;
        }

        public static string GetNonceClearTextV3(DiagnosisSubmissionParameter submission)
        {
            string hasSymptom = submission.HasSymptom ? "HasSymptom" : "NoSymptom";
            return string.Join("|", submission.AppPackageName, submission.OnsetOfSymptomOrTestDate, hasSymptom, GetKeyString(submission.Keys), GetRegionString(submission.Regions), submission.VerificationPayload);

            static string GetKeyString(IEnumerable<DiagnosisSubmissionParameter.Key> keys) =>
                string.Join(",", keys.OrderBy(k => k.KeyData).Select(k => GetKeyStringCore(k)));

            static string GetKeyStringCore(DiagnosisSubmissionParameter.Key k) =>
                string.Join(".", k.KeyData, k.RollingStartNumber, k.RollingPeriod, k.ReportType);

            static string GetRegionString(IEnumerable<string> regions) =>
                string.Join(",", regions.Select(r => r.ToUpperInvariant()).OrderBy(r => r));
        }

        public static string GetNonceClearTextV3(V1EventLogRequest eventLogRequest)
        {
            return string.Join("|", eventLogRequest.IdempotencyKey, eventLogRequest.AppPackageName, GetListClearText(eventLogRequest.EventLogs));

            static string GetListClearText(IList<EventLog> eventLogs)
                => string.Join(",", eventLogs.Select(log => GetClearText(log)));

            static string GetClearText(EventLog log)
                => string.Join(".", log.HasConsent, log.Epoch, log.Type, log.Subtype, log.Content);
        }

        #endregion

        #region V2DiagnosisApi v1.2.2 - v1.4.1

        // For checking compatibility with server API, do not remove this method.
        public static byte[] CreateAndroidNonceV2(DiagnosisSubmissionParameter submission)
        {
            var cleartext = GetNonceClearTextV2(submission);
            var nonce = GetSha256(cleartext);
            return nonce;
        }

        public static string GetNonceClearTextV2(DiagnosisSubmissionParameter submission)
        {
            return string.Join("|", submission.AppPackageName, GetKeyString(submission.Keys), GetRegionString(submission.Regions), submission.VerificationPayload);

            static string GetKeyString(IEnumerable<DiagnosisSubmissionParameter.Key> keys) =>
                string.Join(",", keys.OrderBy(k => k.KeyData).Select(k => GetKeyStringCore(k)));

            static string GetKeyStringCore(DiagnosisSubmissionParameter.Key k) =>
                string.Join(".", k.KeyData, k.RollingStartNumber, k.RollingPeriod);

            static string GetRegionString(IEnumerable<string> regions) =>
                string.Join(",", regions.Select(r => r.ToUpperInvariant()).OrderBy(r => r));
        }
        #endregion

        #region V1DiagnosisApi version v1.0.0 - v1.2.1

        // For checking compatibility with server API, do not remove this method.
        public static byte[] CreateAndroidNonceV1(DiagnosisSubmissionParameter submission)
        {
            var cleartext = GetNonceClearTextV1(submission);
            var nonce = GetSha256(cleartext);
            return nonce;
        }

        public static string GetNonceClearTextV1(DiagnosisSubmissionParameter submission)
        {
            return string.Join("|", submission.AppPackageName, GetKeyString(submission.Keys), GetRegionString(submission.Regions), submission.VerificationPayload);

            static string GetKeyString(IEnumerable<DiagnosisSubmissionParameter.Key> keys) =>
                string.Join(",", keys.OrderBy(k => k.KeyData).Select(k => GetKeyStringCore(k)));

            static string GetKeyStringCore(DiagnosisSubmissionParameter.Key k) =>
                string.Join(".", k.KeyData, k.RollingStartNumber, k.RollingPeriod, 0 /* TransmissionRisk was always 0. */);

            static string GetRegionString(IEnumerable<string> regions) =>
                string.Join(",", regions.Select(r => r.ToUpperInvariant()).OrderBy(r => r));
        }
        #endregion

        private static byte[] GetSha256(string text)
        {
            using var sha = SHA256.Create();
            var textBytes = Encoding.UTF8.GetBytes(text);
            return sha.ComputeHash(textBytes);
        }
    }
}
