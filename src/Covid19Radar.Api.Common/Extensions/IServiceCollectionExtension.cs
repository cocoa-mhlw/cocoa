using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Covid19Radar.Api.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddCosmosClient(this IServiceCollection services)
        {
            return services.AddSingleton<CosmosClient, CosmosClient>((sp) => {
                var config = sp.GetRequiredService<IConfiguration>();
                return new CosmosClient(config.CosmosEndpointUri(), config.CosmosPrimaryKey());
            });
        }
    }
}
