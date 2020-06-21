using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Covid19Radar.Model
{
    public class SelfDiagnosisSubmission 
    {
        public SelfDiagnosisSubmission (bool populatePadding)
        {
            if (populatePadding)
            {
                var random = new Random();
                var size = random.Next(1024, 2048);

                // Approximate the base64 blowup.
                size = (int)(size * 0.75);

                var padding = new byte[size];
                random.NextBytes(padding);
                Padding = Convert.ToBase64String(padding);
            }
        }

        [JsonProperty("submissionNumber")]
        public string SubmissionNumber { get; set; }
        [JsonProperty("userUuid")]
        public string UserUuid { get; set; }
        [JsonProperty("keys")]
        public IEnumerable<ExposureKey> Keys { get; set; } = new List<ExposureKey>();
        // ISO 3166 alpha-2 country code
        [JsonProperty("regions")]
        public IEnumerable<string> Regions { get; set; } = new List<string>();
        // "android" or "ios"
        [JsonProperty("platform")]
        public string Platform { get; set; }
        // The attestation payload for this request. (iOS or Android specific) Base64 encoded.
        [JsonProperty("deviceVerificationPayload")]
        public string DeviceVerificationPayload { get; set; }
        // Some signature / code confirming authorization by the verification authority.
        [JsonProperty("verificationPayload")]
        public string VerificationPayload { get; set; }

        // The identifier for the mobile application. (Android: The App Package AppPackageName, iOS: The BundleID)
        [JsonProperty("appPackageName")]
        public string AppPackageName { get; set; }

        // Random data to obscure the size of the request network packet sniffers.
        [JsonProperty("padding")]
        public string Padding { get; set; }

    }

}
