using Covid19Radar.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Covid19Radar.DataAccess
{
    public interface IOtpRepository
    {
        Task Create(OtpDocument otpDocument);

        Task<OtpDocument?> GetOtpRequestOfUser(string userUuid);

        Task Delete(OtpDocument otpDocument);
    }
}
