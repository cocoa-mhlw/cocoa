using Covid19Radar.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.ExposureNotifications;
using Covid19Radar.Model;
using System.Threading;

namespace Covid19Radar.Model
{
    public class UserDataModel : IEquatable<UserDataModel>
    {

        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        public string UserUuid { get; set; }

        /// <summary>
        /// Secret key
        /// </summary>
        /// <value>Secret Key</value>
        public string Secret { get; set; }

        /// <summary>
        /// Jump Consistent Seed
        /// </summary>
        /// <value>Jump Consistent Seed</value>
        public ulong JumpConsistentSeed { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Last notification date and time
        /// </summary>
        public DateTime LastNotificationTime { get; set; }

        public bool Equals(UserDataModel other)
        {
            return UserUuid == other?.UserUuid
                && LastNotificationTime == other?.LastNotificationTime
                && IsExposureNotificationEnabled == other.IsExposureNotificationEnabled
                && IsNotificationEnabled == other.IsNotificationEnabled;
        }

        /// <summary>
        /// User Unique ID format UserUUID.(Padding Zero Major).(Padding Zero Minor)
        /// </summary>
        /// <value>User Minor</value>
        public string GetId()
        {
            return UserUuid;
        }

        public int GetJumpHashTime()
        {
            return JumpHash.JumpConsistentHash(JumpConsistentSeed, 86400);
        }

        public bool IsOptined { get; set; }

        public bool IsExposureNotificationEnabled { get; set; }

        public bool IsNotificationEnabled { get; set; }

        public bool IsPositived { get; set; }

        public bool IsPolicyAccepted { get; set; }

        public Dictionary<string, long> LastProcessTekTimestamp { get; set; } = new Dictionary<string, long>();

        public Dictionary<string, ulong> ServerBatchNumbers { get; set; } = AppSettings.Instance.GetDefaultDefaultBatch();

        public ObservableCollection<ExposureInfo> ExposureInformation { get; set; } = new ObservableCollection<ExposureInfo>();

        public ExposureDetectionSummary ExposureSummary { get; set; }

        // for mock
        public List<PositiveDiagnosisState> PositiveDiagnoses { get; set; } = new List<PositiveDiagnosisState>();

        public void AddDiagnosis(string diagnosisUid, DateTimeOffset submissionDate)
        {
            if (String.IsNullOrEmpty(diagnosisUid))
            {
                throw new ArgumentNullException();
            }

            var existing = PositiveDiagnoses.Any(d => d.DiagnosisUid.Equals(diagnosisUid, StringComparison.OrdinalIgnoreCase));
            if (existing)
                return;

            // Remove ones that were not submitted as the new one is better
            PositiveDiagnoses.RemoveAll(d => !d.Shared);

            PositiveDiagnoses.Add(new PositiveDiagnosisState
            {
                DiagnosisDate = submissionDate,
                DiagnosisUid = diagnosisUid,
            });
        }

        public void ClearDiagnosis()
            => PositiveDiagnoses.Clear();

        public PositiveDiagnosisState LatestDiagnosis
            => PositiveDiagnoses
                .Where(d => d.Shared)
                .OrderByDescending(p => p.DiagnosisDate)
                .FirstOrDefault();

        public PositiveDiagnosisState PendingDiagnosis
            => PositiveDiagnoses
                .Where(d => !d.Shared)
                .OrderByDescending(p => p.DiagnosisDate)
                .FirstOrDefault();

    }
}
