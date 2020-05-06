using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.ExposureNotifications;

namespace Covid19Radar.Services
{
	public static class LocalStateManager
	{
		static LocalState instance;

		public static LocalState Instance
			=> instance ??= Load() ?? new LocalState();

		static LocalState Load()
		{
			try
			{
				var stateFile = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "localstate.json");
				var data = File.ReadAllText(stateFile);
				return Newtonsoft.Json.JsonConvert.DeserializeObject<LocalState>(data);
			}
			catch
			{
				return new LocalState();
			}
		}

		public static void Save()
		{
			var stateFile = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "localstate.json");
			var data = Newtonsoft.Json.JsonConvert.SerializeObject(Instance);
			File.WriteAllText(stateFile, data);
		}
	}

	public class LocalState
	{
		public bool IsWelcomed { get; set; }
		public bool LastIsEnabled { get; set; } = false;
		public bool EnableNotifications { get; set; } = true;
		public DateTimeOffset NewestKeysResponseTimestamp { get; set; } = DateTimeOffset.MinValue;
		public List<ExposureInfo> ExposureInformation { get; set; } = new List<ExposureInfo>();
		public ExposureDetectionSummary ExposureSummary { get; set; }
		public List<PositiveDiagnosisState> PositiveDiagnoses { get; set; } = new List<PositiveDiagnosisState>();
		PositiveDiagnosisState GetLatest()
		{
			var latest = PositiveDiagnoses?.OrderByDescending(p => p.DiagnosisDate)?.FirstOrDefault();

			if (latest == null)
			{
				latest = new PositiveDiagnosisState();
				PositiveDiagnoses.Add(latest);
			}

			return latest;
		}

		public PositiveDiagnosisState LatestDiagnosis
		{
			get => GetLatest();
			set
			{
				var latest = GetLatest();
				latest.DiagnosisDate = value.DiagnosisDate;
				latest.DiagnosisUid = value.DiagnosisUid;
				latest.Shared = value.Shared;
			}
		}
	}

	public class PositiveDiagnosisState
	{
		public string DiagnosisUid { get; set; }

		public DateTimeOffset DiagnosisDate { get; set; }

		// Set true after actually submitted to server
		public bool Shared { get; set; } = false;
	}
}
