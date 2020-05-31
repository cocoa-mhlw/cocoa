using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ExposureNotification.Backend.Database;
using ExposureNotification.Backend.Proto;
using ExposureNotification.Backend.Signing;
using Xunit;

namespace ExposureNotification.Backend.Functions.Tests
{
	public class ExposureBatchFileUtilTests
	{
		[Fact]
		public async Task ValidateFile()
		{
			var expectedExport = GenerateRandomExport();
			using var stream = await ExposureBatchFileUtil.CreateSignedFileAsync(expectedExport, TestSignatures);

			using var zipFile = new ZipArchive(stream);

			var expectedEntries = new[] { "export.bin", "export.sig" };
			var entries = zipFile.Entries.Select(e => e.FullName);
			Assert.Equal(expectedEntries, entries);
		}

		[Fact]
		public async Task ValidateFileBinary()
		{
			var expectedExport = GenerateRandomExport();
			using var stream = await ExposureBatchFileUtil.CreateSignedFileAsync(expectedExport, TestSignatures);

			using var zipFile = new ZipArchive(stream);
			using var exportBin = zipFile.GetBin();

			var export = TemporaryExposureKeyExport.Parser.ParseFrom(exportBin);
			Assert.NotNull(export);

			var info = Assert.Single(export.SignatureInfos);
			Assert.Equal("1.2.840.10045.4.3.2", info.SignatureAlgorithm);
			Assert.Equal("TestServer", info.VerificationKeyId);
			Assert.Equal("2", info.VerificationKeyVersion);
		}

		[Fact]
		public async Task ValidateFileSignature()
		{
			var expectedExport = GenerateRandomExport();
			using var stream = await ExposureBatchFileUtil.CreateSignedFileAsync(expectedExport, TestSignatures);

			using var zipFile = new ZipArchive(stream);
			using var exportSig = zipFile.GetSignature();

			var signatureList = TEKSignatureList.Parser.ParseFrom(exportSig);
			Assert.NotNull(signatureList);

			var signature = Assert.Single(signatureList.Signatures);
			Assert.NotEmpty(signature.Signature.ToByteArray());

			var info = signature.SignatureInfo;
			Assert.Equal("1.2.840.10045.4.3.2", info.SignatureAlgorithm);
			Assert.Equal("TestServer", info.VerificationKeyId);
			Assert.Equal("2", info.VerificationKeyVersion);
		}

		[Fact]
		public async Task CanCreateStream()
		{
			var export = Utils.GenerateTemporaryExposureKeyExport(10);

			using var stream = await ExposureBatchFileUtil.CreateSignedFileAsync(export, TestSignatures);

			Assert.NotNull(stream);
			Assert.NotEqual(0, stream.Length);
		}

		[Fact]
		public async Task CreateSignedFileAddsHeaderToBin()
		{
			var export = Utils.GenerateTemporaryExposureKeyExport(10);

			using var stream = await ExposureBatchFileUtil.CreateSignedFileAsync(export, TestSignatures);
			using var zip = new ZipArchive(stream);

			using var bin = zip.GetBin(false);

			var header = new byte[16];
			bin.Read(header);
			Assert.Equal(TemporaryExposureKeyExport.Header, header);
		}

		[Fact]
		public void CreateExportSetsTimestampsCorrectly()
		{
			var date = DateTimeOffset.Now;
			var keys = new[]
			{
				Utils.GenerateRandomDbKey(date),
				Utils.GenerateRandomDbKey(date.AddMinutes(-1)),
				Utils.GenerateRandomDbKey(date.AddDays(-2)),
				Utils.GenerateRandomDbKey(date.AddHours(-6)),
				Utils.GenerateRandomDbKey(date.AddMilliseconds(-1)),
			};

			var export = ExposureNotificationStorage.CreateUnsignedExport("ZA", 1, 1, keys);
			Assert.NotNull(export);

			Assert.Equal((ulong)date.AddDays(-2).ToUnixTimeSeconds(), export.StartTimestamp);
			Assert.Equal((ulong)date.ToUnixTimeSeconds(), export.EndTimestamp);
		}

		[Fact]
		public async Task CanCreateStreamFromDbItems()
		{
			var expectedExport = GenerateRandomExport();

			using var stream = await ExposureBatchFileUtil.CreateSignedFileAsync(expectedExport, TestSignatures);
			Assert.NotNull(stream);

			using var zip = new ZipArchive(stream);
			using var exportBin = zip.GetBin();

			var actualExport = TemporaryExposureKeyExport.Parser.ParseFrom(exportBin);

			Assert.Equal(expectedExport.Keys, actualExport.Keys);
		}

		//[Fact]
		//public async Task ValidateSignedExampleSignatureValidity()
		//{
		//	var export = GenerateRandomExport();
		//	using var stream = await ExposureBatchFileUtil.CreateSignedFileAsync(export, TestSignatures, Signer);

		//	Utils.ValidateExportFileSignature(stream, TestPublicKey);
		//}

		static SignerInfoConfig[] TestSignatures
			=> new SignerInfoConfig[]
			{
				new SignerInfoConfig
				{
					AndroidPackage = "com.xamarin.exposurenotificationsample.tests",
					AppBundleId = "com.xamarin.exposurenotificationsample.tests",
					VerificationKeyId = "TestServer",
					VerificationKeyVersion = "2",
					SigningKeyBase64String = Convert.ToBase64String(File.ReadAllBytes("TestAssets/sample-ecdsa-p256-cert.pfx"))
				}
			};

		public string TestPublicKey
			=> @"
-----BEGIN PUBLIC KEY-----
MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEUczyMAkfSeoU77Nmcb1G7t7xyGCA
hQqMOIVDFLFas3J+elP7CiotovigCLWj706F07j1EPL27ThRzZl7Ha9uOA==
-----END PUBLIC KEY-----";

		static TemporaryExposureKeyExport GenerateRandomExport()
		{
			var date = DateTimeOffset.Now;
			var keys = new[]
			{
				Utils.GenerateRandomDbKey(date),
				Utils.GenerateRandomDbKey(date.AddMinutes(-1)),
				Utils.GenerateRandomDbKey(date.AddDays(-2)),
				Utils.GenerateRandomDbKey(date.AddHours(-6)),
				Utils.GenerateRandomDbKey(date.AddMilliseconds(-1)),
			};

			return ExposureNotificationStorage.CreateUnsignedExport("ZA", 1, 1, keys);
		}
	}
}
