using Covid19Radar.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.ExposureNotifications;

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
        /// Jump Consistent Hash 
        /// </summary>
        /// <value>Jump Consistent Hash</value>
        public ulong JumpConsistentHash { get; set; }

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
            return JumpHash.JumpConsistentHash(JumpConsistentHash, AppConstants.NumberOfGroup);
        }

        // From Xamarin.EN LocalState Class
        public static readonly Dictionary<string, ulong> DefaultServerBatchNumbers = new Dictionary<string, ulong> { { "ZA", 0 }, { "CA", 0 } };

        public bool IsWelcomed { get; set; }

        public bool LastIsEnabled { get; set; } = false;

        public bool EnableNotifications { get; set; } = true;
        public long ServerLastTime { get; set; } = 0;
        public string Region { get; set; } = AppConstants.DefaultRegion;

        public Dictionary<string, ulong> ServerBatchNumbers { get; set; } = new Dictionary<string, ulong>(DefaultServerBatchNumbers);

        public ObservableCollection<ExposureInfo> ExposureInformation { get; set; } = new ObservableCollection<ExposureInfo>();

        public ExposureDetectionSummary ExposureSummary { get; set; }

        public List<PositiveDiagnosisStateModel> PositiveDiagnoses { get; set; } = new List<PositiveDiagnosisStateModel>();

        public void AddDiagnosis(string diagnosisUid, DateTimeOffset submissionDate)
        {
            var existing = PositiveDiagnoses?.Where(d => d.DiagnosisUid.Equals(diagnosisUid, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(d => d.DiagnosisDate).FirstOrDefault();

            if (existing != null)
                return;

            PositiveDiagnoses.Add(new PositiveDiagnosisStateModel
            {
                DiagnosisDate = submissionDate,
                DiagnosisUid = diagnosisUid,
            });
        }

        public void ClearDiagnosis()
            => PositiveDiagnoses?.Clear();

        public PositiveDiagnosisStateModel LatestDiagnosis
            => PositiveDiagnoses?
                .Where(d => d.Shared)
                .OrderByDescending(p => p.DiagnosisDate)?
                .FirstOrDefault();

        public PositiveDiagnosisStateModel PendingDiagnosis
            => PositiveDiagnoses?
                .Where(d => !d.Shared)
                .OrderByDescending(p => p.DiagnosisDate)?
                .FirstOrDefault();

    }
}
