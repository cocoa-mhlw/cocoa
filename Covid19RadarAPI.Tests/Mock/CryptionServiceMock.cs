using Covid19Radar.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Tests.Mock
{
    public class CryptionServiceMock : ICryptionService
    {
        public string CreateSecret()
        {
            return "SECRET";
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
