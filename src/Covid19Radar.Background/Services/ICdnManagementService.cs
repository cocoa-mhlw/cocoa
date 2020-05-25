using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public interface ICdnManagementService
    {
        Task PurgeAsync(IList<string> contentPaths);
    }
}
