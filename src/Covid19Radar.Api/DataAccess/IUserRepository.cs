using Covid19Radar.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Covid19Radar.DataAccess
{
    public interface IUserRepository
    {
        Task<UserResultModel?> GetById(string id);

        Task Create(UserModel user);

        Task<bool> Exists(string id);

        Task<SequenceDataModel?> NextSequenceNumber();
    }
}
