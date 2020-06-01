using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExposureNotification.Backend.Database
{
	public class SignerInfoConfig
	{
		public string AndroidPackage { get; set; }

		public string AppBundleId { get; set; }

		public string VerificationKeyId { get; set; }

		public string VerificationKeyVersion { get; set; }

		public string SigningKeyBase64String { get; set; }
	}
}
