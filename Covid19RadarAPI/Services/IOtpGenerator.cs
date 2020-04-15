using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Services
{
    public interface IOtpGenerator
    {
        string Generate(DateTime timeStamp);
        bool Validate(string code, DateTime timeStamp);
    }
}
