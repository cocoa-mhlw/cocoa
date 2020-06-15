using Covid19Radar.Api.Models;
using System;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public interface IDiagnosisRepository
    {

        Task<DiagnosisModel[]> GetNotApprovedAsync();
        Task<DiagnosisModel> GetAsync(string SubmissionNumber, string UserUuid);

        Task<DiagnosisModel> SubmitDiagnosisAsync(string SubmissionNumber, DateTimeOffset timestamp, string UserUuid, TemporaryExposureKeyModel[] Keys);

        Task DeleteAsync(IUser user);
    }
}
