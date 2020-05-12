using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Services
{
    public interface ICryptionService
    {
        string CreateSecret(string userUuid);
        bool ValidateSecret(string userUuid, string secret);
        string Protect(string secret);
        string Unprotect(string protectSecret);
    }
}
