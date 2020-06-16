using Covid19Radar.Api.Models;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public interface IAuthorizedAppRepository
    {
        Task<AuthorizedAppInformation> GetAsync(string platform);
    }
}
