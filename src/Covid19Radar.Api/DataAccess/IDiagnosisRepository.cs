using Covid19Radar.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.DataAccess
{
    public interface IDiagnosisRepository
    {
        Task SubmitDiagnosisAsync(DiagnosisSubmissionParameter Parameter);

        Task Delete(IUser user);
    }
}
