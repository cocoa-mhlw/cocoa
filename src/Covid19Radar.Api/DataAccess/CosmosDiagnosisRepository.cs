using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.DataAccess
{
    public class CosmosDiagnosisRepository : IDiagnosisRepository
    {
        private readonly ICosmos _db;
        private readonly ILogger<CosmosDiagnosisRepository> _logger;

        public CosmosDiagnosisRepository(ICosmos db, ILogger<CosmosDiagnosisRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public Task SubmitDiagnosisAsync(DiagnosisSubmissionParameter Parameter)
        {
            var item = new DiagnosisModel()
            {
                id = Parameter.UserUuid,
                PartitionKey = Parameter.SubmissionNumber,
                UserUuid = Parameter.UserUuid,
                Keys = Parameter.Keys
            };

            return _db.Diagnosis.UpsertItemAsync<DiagnosisModel>(item, new PartitionKey(item.PartitionKey));
        }
    }
}
