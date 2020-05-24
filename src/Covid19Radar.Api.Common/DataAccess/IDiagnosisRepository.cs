using Covid19Radar.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public interface IDiagnosisRepository
    {

        Task<DiagnosisModel[]> GetNotApprovedAsync();
        Task<DiagnosisModel> GetAsync(string SubmissionNumber, string UserUuid);

        Task SubmitDiagnosisAsync(string SubmissionNumber, DateTimeOffset timestamp, string UserUuid, TemporaryExposureKeyModel[] Keys);

        Task Delete(IUser user);
    }
}
