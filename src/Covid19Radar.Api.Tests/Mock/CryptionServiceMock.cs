using Covid19Radar.Api.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Api.Tests.Mock
{
    public class CryptionServiceMock : ICryptionService
    {
        public string CreateSecret(string userUuid)
        {
            return "SECRET";
        }

        public bool ValidateSecret(string userUuid, string secret)
        {
            return true;
        }

        public string Protect(string secret)
        {
            return "PROTECT_SECRET";
        }

        public string Unprotect(string protectSecret)
        {
            return "SECRET";
        }

    }
}
