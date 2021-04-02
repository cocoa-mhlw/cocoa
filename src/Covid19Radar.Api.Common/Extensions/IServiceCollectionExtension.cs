/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
                return new CosmosClient(config.CosmosEndpointUri(),
                                        config.CosmosPrimaryKey(),
                                        new CosmosClientOptions() { 
                                            ConnectionMode = ConnectionMode.Direct,
                                            MaxRequestsPerTcpConnection = 16,

                                        });
            });
        }
    }
}
