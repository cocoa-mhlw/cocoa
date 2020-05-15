using Covid19Radar.Common;
using Covid19Radar.Protobuf;
using Google.Protobuf;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
	public class TemporaryExposureKeyModel
	{
		public string id { get; set; } = Guid.NewGuid().ToString();

		public byte[] KeyData { get; set; }
		public int RollingPeriod { get; }
		public int RollingStartIntervalNumber { get; }
		public int TransmissionRiskLevel { get; }

		public TemporaryExposureKey ToKey()
		{
			return new TemporaryExposureKey()
			{
				KeyData = ByteString.CopyFrom(KeyData),
				RollingStartIntervalNumber = RollingStartIntervalNumber,
				RollingPeriod = RollingPeriod,
				TransmissionRiskLevel = TransmissionRiskLevel
			};
		}
	}

}
