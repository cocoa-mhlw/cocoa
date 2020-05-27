using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public class CosmosAuthorizedApp : IAuthorizedAppRepository
    {
        private readonly ICosmos _db;
        private readonly ILogger<CosmosAuthorizedApp> _logger;
        public CosmosAuthorizedApp(ICosmos db, ILogger<CosmosAuthorizedApp> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<AuthorizedApp> GetAsync(string platform)
        {
            _logger.LogInformation($"start {nameof(GetAsync)}");
            var response = await _db.AuthorizedApp.ReadItemAsync<AuthorizedApp>(platform, PartitionKey.None);
            return response.Resource;
        }
    }
}
