using Covid19Radar.Api.Models;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public interface ICustomVerificationStatusRepository
    {
        Task<CustomVerificationStatusModel[]> GetAsync();

    }
}
