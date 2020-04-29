using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using OtpNet;

namespace Covid19Radar.Services
{
    public class OtpGenerator : IOtpGenerator
    {
        private const string TotpSecretKeySetting = "TOTP_SECRET";
        private readonly string _otpSecretKey;

        public OtpGenerator(IConfiguration configuration)
        {
            _otpSecretKey = configuration[TotpSecretKeySetting];
        }
        public string Generate(DateTime timeStamp)
        {
            var generator = new Totp(Encoding.UTF8.GetBytes(_otpSecretKey));
            return generator.ComputeTotp(timeStamp);
        }

        public bool Validate(string code, DateTime timeStamp)
        {
            var generator = new Totp(Encoding.UTF8.GetBytes(_otpSecretKey));
            var valid = generator.VerifyTotp(timeStamp, code, out long timeStepMatched);
            return valid;
        }
    }
}
