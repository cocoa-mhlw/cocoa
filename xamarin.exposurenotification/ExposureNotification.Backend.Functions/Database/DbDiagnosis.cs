using System.ComponentModel.DataAnnotations;

namespace ExposureNotification.Backend.Database
{
	public class DbDiagnosis
	{
		public DbDiagnosis(string diagnosisUid)
			=> DiagnosisUid = diagnosisUid;

		[Key]
		public string DiagnosisUid { get; set; }

		public int KeyCount { get; set; }
	}
}
