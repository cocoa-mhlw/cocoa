using System.Text;

namespace ExposureNotification.Backend.Functions
{
	public class Settings
	{
		public string DbConnectionString { get; set; }

		public string BlobStorageConnectionString { get; set; }

		public string BlobStorageContainerNamePrefix { get; set; }

		public string[] SupportedRegions { get; set; }

		public bool DeleteKeysFromDbAfterBatching { get; set; }

		public bool DisableDeviceVerification { get; set; }

		public string SigningKeyBase64String { get; set; }
		public string VerificationKeyId { get; set; }
		public string VerificationKeyVersion { get; set; }

		public string AndroidPackageName { get; set; }
		public string iOSBundleId { get; set; }
		public string iOSDeviceCheckKeyId { get; set; }
		public string iOSDeviceCheckTeamId { get; set; }
		public string iOSDeviceCheckPrivateKey { get; set; }


		public override string ToString()
		{
			string WriteSecret(string s)
				=> string.IsNullOrEmpty(s) ? "NULL" : "<hidden>";

			var sb = new StringBuilder();

			sb.AppendLine($"DbConnectionString : {WriteSecret(DbConnectionString)}");
			sb.AppendLine($"BlobStorageConnectionString : {WriteSecret(BlobStorageConnectionString)}");
			sb.AppendLine($"BlobStorageContainerNamePrefix : {BlobStorageContainerNamePrefix}");
			sb.AppendLine($"SupportedRegions: {string.Join(';', SupportedRegions)}");
			sb.AppendLine($"SigningKeyBase64String : {WriteSecret(SigningKeyBase64String)}");
			sb.AppendLine($"VerificationKeyId: {VerificationKeyId}");
			sb.AppendLine($"VerificationKeyVersion : {VerificationKeyVersion}");
			sb.AppendLine($"DeleteKeysFromDbAfterBatching : {DeleteKeysFromDbAfterBatching}");
			sb.AppendLine($"DisableDeviceVerification : {DisableDeviceVerification}");
			sb.AppendLine($"AndroidPackageName : {AndroidPackageName}");
			sb.AppendLine($"iOSBundleId : {iOSBundleId}");
			sb.AppendLine($"iOSDeviceCheckKeyId : {iOSDeviceCheckKeyId}");
			sb.AppendLine($"iOSDeviceCheckPrivateKey : {WriteSecret(iOSDeviceCheckPrivateKey)}");
			sb.AppendLine($"iOSDeviceCheckTeamId : {iOSDeviceCheckTeamId}");

			return sb.ToString();
		}
	}
}
