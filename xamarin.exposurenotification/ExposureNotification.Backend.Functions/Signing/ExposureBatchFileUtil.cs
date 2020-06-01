using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ExposureNotification.Backend.Database;
using ExposureNotification.Backend.Proto;
using Google.Protobuf;

namespace ExposureNotification.Backend.Signing
{
	public static class ExposureBatchFileUtil
	{
		public const string SignatureAlgorithm = "1.2.840.10045.4.3.2";
		public const string BinEntryName = "export.bin";
		public const string SigEntryName = "export.sig";

		public static async Task<Stream> CreateSignedFileAsync(TemporaryExposureKeyExport export, IEnumerable<SignerInfoConfig> signerInfos)
		{
			export.SignatureInfos.AddRange(signerInfos.Select(sigInfo => new SignatureInfo
			{
				AndroidPackage = sigInfo.AndroidPackage,
				AppBundleId = sigInfo.AppBundleId,
				SignatureAlgorithm = SignatureAlgorithm,
				VerificationKeyId = sigInfo.VerificationKeyId,
				VerificationKeyVersion = sigInfo.VerificationKeyVersion,
			}));

			var ms = new MemoryStream();

			using (var zipFile = new ZipArchive(ms, ZipArchiveMode.Create, true))
			using (var bin = await CreateBinAsync(export))
			using (var sig = await CreateSigAsync(export, bin.ToArray(), signerInfos))
			{
				// Copy the bin contents into the entry
				var binEntry = zipFile.CreateEntry(BinEntryName, CompressionLevel.Optimal);
				using (var binStream = binEntry.Open())
				{
					await bin.CopyToAsync(binStream);
				}

				// Copy the sig contents into the entry
				var sigEntry = zipFile.CreateEntry(SigEntryName, CompressionLevel.NoCompression);
				using (var sigStream = sigEntry.Open())
				{
					await sig.CopyToAsync(sigStream);
				}
			}

			// Rewind to the front for the consumer
			ms.Position = 0;
			return ms;
		}

		public static async Task<MemoryStream> CreateBinAsync(TemporaryExposureKeyExport export)
		{
			var stream = new MemoryStream();

			// Write header
			await stream.WriteAsync(TemporaryExposureKeyExport.Header);

			// Write export proto
			export.WriteTo(stream);

			// Rewind to the front for the consumer
			stream.Position = 0;

			return stream;
		}

		public static async Task<MemoryStream> CreateSigAsync(TemporaryExposureKeyExport export, byte[] exportBytes, IEnumerable<SignerInfoConfig> signerInfos)
		{
			var stream = new MemoryStream();

			// Create signature list object
			var tk = new TEKSignatureList();
			foreach (var sigInfo in signerInfos)
			{
				// Generate the signature from the bin file contents
				var sig = await GenerateSignatureAsync(exportBytes, sigInfo);

				tk.Signatures.Add(new TEKSignature
				{
					BatchNum = export.BatchNum,
					BatchSize = export.BatchSize,
					SignatureInfo = new SignatureInfo
					{
						AndroidPackage = sigInfo.AndroidPackage,
						AppBundleId = sigInfo.AppBundleId,
						SignatureAlgorithm = SignatureAlgorithm,
						VerificationKeyId = sigInfo.VerificationKeyId,
						VerificationKeyVersion = sigInfo.VerificationKeyVersion,
					},
					Signature = ByteString.CopyFrom(sig),
				});
			}

			// Write signature proto
			tk.WriteTo(stream);

			// Rewind to the front for the consumer
			stream.Position = 0;

			return stream;
		}

		public static Task<byte[]> GenerateSignatureAsync(byte[] contents, SignerInfoConfig signerInfo)
		{
			// This is actually am Elliptic Curve certificate (ECDSA) with a P-256 curve
			// It's been encoded to a base64 string
			// Turn this into a certificate object
			var bytes = Convert.FromBase64String(signerInfo.SigningKeyBase64String);
			var keyVaultCert = new X509Certificate2(bytes);

			// Get the private key to use for creating the signature
			var ecdsaPrivateKey = keyVaultCert.GetECDsaPrivateKey();

			// Create our signature based on the contents
			var signature = ecdsaPrivateKey.SignData(contents, HashAlgorithmName.SHA256);

			return Task.FromResult(signature);
		}
	}
}
