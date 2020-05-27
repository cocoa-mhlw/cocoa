using Covid19Radar.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public interface IAuthorizedAppRepository
    {
        Task<AuthorizedApp> GetAsync(string platform);
    }
}
