﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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

        public async Task DeleteAsync(IUser user)
        {
            _logger.LogInformation($"start {nameof(DeleteAsync)}");
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
            var q = new QueryDefinition("SELECT * FROM c WHERE c.Approved == false");
            var query = _db.Diagnosis.GetItemQueryIterator<DiagnosisModel>(q);
            var e = Enumerable.Empty<DiagnosisModel>();
            while (query.HasMoreResults)
            {
                var r = await query.ReadNextAsync();
                e = e.Concat(r.Resource);
            }

            return e.ToArray();
        }

        public async Task<DiagnosisModel> SubmitDiagnosisAsync(string SubmissionNumber, DateTimeOffset timestamp, string UserUuid, TemporaryExposureKeyModel[] Keys)
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

            return (await _db.Diagnosis.UpsertItemAsync<DiagnosisModel>(item)).Resource;
        }
    }
}
