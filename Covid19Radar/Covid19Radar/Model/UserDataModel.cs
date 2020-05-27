using Covid19Radar.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.ExposureNotifications;
using Covid19Radar.Model;

namespace Covid19Radar.Model
{
    public class UserDataModel: IEquatable<UserDataModel>
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
        /// Last notification date and time
        /// </summary>
        public DateTime LastNotificationTime { get; set; }

        public bool Equals(UserDataModel other)
        {
            return UserUuid == other?.UserUuid
                && LastNotificationTime == other?.LastNotificationTime;
        }

        /// <summary>
        /// User Unique ID format UserUUID.(Padding Zero Major).(Padding Zero Minor)
        /// </summary>
        /// <value>User Minor</value>
        public string GetId()
        {
            return UserUuid;
        }

        public int GetJumpHashTimeDifference()
        {
            return JumpHash.JumpConsistentHash(JumpConsistentSeed, AppConstants.NumberOfGroup);
        }

        // From Xamarin.EN LocalState Class
        public static readonly Dictionary<string, ulong> DefaultServerBatchNumbers = new Dictionary<string, ulong> { { "ZA", 0 }, { "CA", 0 } };

        public bool IsWelcomed { get; set; } = false;

        public bool IsExposureNotificationEnabled { get; set; } = false;

        public bool IsNotificationEnabled { get; set; } = true;
        
        //public long ServerLastTime { get; set; } = 0;

        //public string Region { get; set; } = AppConstants.DefaultRegion;

        public Dictionary<string, ulong> ServerBatchNumbers { get; set; } = new Dictionary<string, ulong>(DefaultServerBatchNumbers);

        public ObservableCollection<ExposureInfo> ExposureInformation { get; set; } = new ObservableCollection<ExposureInfo>();

        public ExposureDetectionSummary ExposureSummary { get; set; }

        public List<PositiveDiagnosisState> PositiveDiagnoses { get; set; } = new List<PositiveDiagnosisState>();


        public void AddDiagnosis(string diagnosisUid, DateTimeOffset submissionDate)
        {
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
