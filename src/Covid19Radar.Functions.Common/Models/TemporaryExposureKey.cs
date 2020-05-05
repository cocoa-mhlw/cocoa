using Covid19Radar.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
	public class TemporaryExposureKey
	{
		public string id { get; set; } = Guid.NewGuid().ToString();

		public string Base64KeyData { get; set; }

		public long TimestampSecondsSinceEpoch { get; set; }

		public long RollingStartSecondsSinceEpoch { get; set; }

		public int RollingDuration { get; set; }

		public int TransmissionRiskLevel { get; set; }


	}

}
