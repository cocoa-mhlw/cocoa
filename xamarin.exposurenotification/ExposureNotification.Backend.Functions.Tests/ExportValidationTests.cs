using System.IO;
using System.IO.Compression;
using System.Linq;
using ExposureNotification.Backend.Proto;
using Xunit;

namespace ExposureNotification.Backend.Functions.Tests
{
	public class ExportValidationTests
	{
		[Fact]
		public void ValidateSignedExample()
		{
			using var zipFile = ZipFile.OpenRead("TestAssets/SignedExample/export.zip");

			var expectedEntries = new[] { "export.bin", "export.sig" };
			var entries = zipFile.Entries.Select(e => e.FullName);
			Assert.Equal(expectedEntries, entries);
		}

		[Fact]
		public void ValidateSignedExampleBinary()
		{
			using var zipFile = ZipFile.OpenRead("TestAssets/SignedExample/export.zip");
			using var exportBin = zipFile.GetBin();

			var export = TemporaryExposureKeyExport.Parser.ParseFrom(exportBin);
			Assert.NotNull(export);

			var info = Assert.Single(export.SignatureInfos);
			Assert.Equal("1.2.840.10045.4.3.2", info.SignatureAlgorithm);
			Assert.Equal("ExampleServer_k1", info.VerificationKeyId);
		}

		[Fact]
		public void ValidateSignedExampleSignature()
		{
			using var zipFile = ZipFile.OpenRead("TestAssets/SignedExample/export.zip");
			using var exportSig = zipFile.GetSignature();

			var signatureList = TEKSignatureList.Parser.ParseFrom(exportSig);
			Assert.NotNull(signatureList);

			var signature = Assert.Single(signatureList.Signatures);
			Assert.NotEmpty(signature.Signature.ToByteArray());

			var info = signature.SignatureInfo;
			Assert.Equal("1.2.840.10045.4.3.2", info.SignatureAlgorithm);
			Assert.Equal("ExampleServer_k1", info.VerificationKeyId);
		}

		[Theory]
		[InlineData("TestAssets/SignedExample/export.zip", "TestAssets/SignedExample/public.pem")]
		public void ValidateSignedExampleSignatureValidity(string zipPath, string pemPath)
		{
			using var zip = File.OpenRead(zipPath);
			var pem = File.ReadAllText(pemPath);

			Utils.ValidateExportFileSignature(zip, pem);
		}

		[Fact]
		public void ValidateSignedExample2()
		{
			using var zipFile = ZipFile.OpenRead("TestAssets/SignedExample2/export.zip");

			var expectedEntries = new[] { "export.bin", "export.sig" };
			var entries = zipFile.Entries.Select(e => e.FullName);
			Assert.Equal(expectedEntries, entries);
		}

		[Fact]
		public void ValidateSignedExample2Binary()
		{
			using var zipFile = ZipFile.OpenRead("TestAssets/SignedExample2/export.zip");
			using var exportBin = zipFile.GetBin();

			var export = TemporaryExposureKeyExport.Parser.ParseFrom(exportBin);
			Assert.NotNull(export);

			var info = Assert.Single(export.SignatureInfos);
			Assert.Equal("1.2.840.10045.4.3.2", info.SignatureAlgorithm);
			Assert.Equal("some_id", info.VerificationKeyId);
		}

		[Fact]
		public void ValidateSignedExample2Signature()
		{
			using var zipFile = ZipFile.OpenRead("TestAssets/SignedExample2/export.zip");
			using var exportSig = zipFile.GetSignature();

			var signatureList = TEKSignatureList.Parser.ParseFrom(exportSig);
			Assert.NotNull(signatureList);

			var signature = Assert.Single(signatureList.Signatures);
			Assert.NotEmpty(signature.Signature.ToByteArray());

			var info = signature.SignatureInfo;
			Assert.Equal("1.2.840.10045.4.3.2", info.SignatureAlgorithm);
			Assert.Equal("some_id", info.VerificationKeyId);
		}

		[Theory]
		[InlineData("TestAssets/SignedExample2/export.zip", "TestAssets/SignedExample2/public.pem")]
		public void ValidateSignedExample2SignatureValidity(string zipPath, string pemPath)
		{
			using var zip = File.OpenRead(zipPath);
			var pem = File.ReadAllText(pemPath);

			Utils.ValidateExportFileSignature(zip, pem);
		}

		[Fact]
		public void ValidateKeysExample()
		{
			using var zipFile = ZipFile.OpenRead("TestAssets/KeysExample/export.zip");

			var expectedEntries = new[] { "export.bin", "export.sig" };
			var entries = zipFile.Entries.Select(e => e.FullName);
			Assert.Equal(expectedEntries, entries);

			using var exportBin = zipFile.GetBin();

			var export = TemporaryExposureKeyExport.Parser.ParseFrom(exportBin);
			Assert.NotNull(export);
			Assert.Equal(110, export.Keys.Count);

			var info = Assert.Single(export.SignatureInfos);
			Assert.Equal("com.google.android.apps.exposurenotification", info.AndroidPackage);
		}
	}
}
