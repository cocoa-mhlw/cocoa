using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Covid19Radar.Models;

namespace Covid19Radar.Services
{
    public interface IOtpService
    {
        Task SendAsync(OtpSendRequest request);
    }
}
