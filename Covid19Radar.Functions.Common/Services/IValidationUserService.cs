using Covid19Radar.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface IValidationUserService
    {

        Task<bool> ValidateAsync(HttpRequest req, IUser user);
    }
}
