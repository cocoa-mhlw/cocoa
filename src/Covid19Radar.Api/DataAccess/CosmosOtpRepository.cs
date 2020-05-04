using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Covid19Radar.DataAccess
{
    public class CosmosOtpRepository : IOtpRepository
    {
        private readonly ICosmos _db;

        public CosmosOtpRepository(ICosmos db)
        {
            _db = db;
        }

        public Task Create(OtpDocument otpDocument)
        {
            return _db.Otp.CreateItemAsync(otpDocument, new PartitionKey(otpDocument.UserUuid));
        }

        public async Task<OtpDocument?> GetOtpRequestOfUser(string userUuid)
        {
            OtpDocument? otpDocument = null;
            try
            {
                var iterator = _db.Otp.GetItemLinqQueryable<OtpDocument>()
                        .Where(o => o.UserUuid == userUuid)
                        .ToFeedIterator();

                if (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    otpDocument = response.FirstOrDefault();
                }
            }
            catch (CosmosException cosmosException)
            {
                if (cosmosException.StatusCode == HttpStatusCode.NotFound)
                {
                    otpDocument = null;
                }
            }

            return otpDocument;
        }

        public Task Delete(OtpDocument otpDocument)
        {
            return _db.Otp.DeleteItemAsync<OtpDocument>(otpDocument.id, new PartitionKey(otpDocument.UserUuid));
        }
    }
}
