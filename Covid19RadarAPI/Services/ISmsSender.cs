using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface ISmsSender
    {
        Task<bool> SendAsync(string body, string toPhone);

    }
}
