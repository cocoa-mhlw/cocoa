using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
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

        public async Task Delete(IUser user)
        {
            _logger.LogInformation($"start {nameof(Delete)}");
            await _db.Diagnosis.DeleteItemAsync<DiagnosisModel>(user.GetId(), PartitionKey.None);
        }

        public async Task<DiagnosisModel> GetAsync(string submissionNumber, string userUuid)
        {
            _logger.LogInformation($"start {nameof(GetAsync)}");
            var response = await _db.Diagnosis.ReadItemAsync<DiagnosisModel>(userUuid, PartitionKey.None);
            return response.Resource;
        }

        public async Task<DiagnosisModel[]> GetNotApprovedAsync()
        {
            _logger.LogInformation($"start {nameof(GetNotApprovedAsync)}");
            var query = _db.Diagnosis.GetItemLinqQueryable<DiagnosisModel>()
                   .Where(_ => _.Approved == false)
                   .ToFeedIterator();
            var e = Enumerable.Empty<DiagnosisModel>();
            while (query.HasMoreResults)
            {
                var r = await query.ReadNextAsync();
                e = e.Concat(r.Resource);
            }

            return e.ToArray();
        }

        public Task SubmitDiagnosisAsync(string SubmissionNumber, DateTimeOffset timestamp, string UserUuid, TemporaryExposureKeyModel[] Keys)
        {
            _logger.LogInformation($"start {nameof(SubmitDiagnosisAsync)}");
            var item = new DiagnosisModel()
            {
                id = UserUuid,
                UserUuid = UserUuid,
                SubmissionNumber = SubmissionNumber,
                Approved = false,
                Timestamp = timestamp.ToUnixTimeSeconds(),
                Keys = Keys
            };

            return _db.Diagnosis.UpsertItemAsync<DiagnosisModel>(item, PartitionKey.None);
        }
    }
}
