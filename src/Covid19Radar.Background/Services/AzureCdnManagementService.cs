using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Cdn.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;

namespace Covid19Radar.Background.Services
{
    public class AzureCdnManagementService : ICdnManagementService
    {
        private readonly ILogger<AzureCdnManagementService> Logger;
        private readonly CdnManagementClient CdnClient;
        private readonly string CdnResourceGroupName;
        private readonly string CdnProfileName;
        private readonly string CdnEndpointName;

        public AzureCdnManagementService(
            IConfiguration config,
            ILogger<AzureCdnManagementService> logger
            )
        {
            Logger = logger;
            Logger.LogInformation($"{nameof(AzureCdnManagementService)} constructor");
            CdnResourceGroupName = config.CdnResourceGroupName();
            CdnProfileName = config.CdnProfileName();
            CdnEndpointName = config.CdnEndpointName();
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            var mslogin = new MSILoginInformation(MSIResourceType.AppService);
            var credential = new AzureCredentials(mslogin, AzureEnvironment.AzureGlobalCloud);
            var client = RestClient.Configure()
                .WithEnvironment(AzureEnvironment.AzureGlobalCloud)
                .WithCredentials(credential)
                .Build();
            CdnClient = new CdnManagementClient(client);
        }


        public async Task PurgeAsync(IList<string> contentPaths)
        {
            Logger.LogInformation($"start {nameof(PurgeAsync)}");
            await CdnClient.Endpoints.PurgeContentAsync(CdnResourceGroupName, CdnProfileName, CdnEndpointName, contentPaths);
        }

        public async Task LoadContentAsync(IList<string> contentPaths)
        {
            Logger.LogInformation($"start {nameof(LoadContentAsync)}");
            await CdnClient.Endpoints.LoadContentAsync(CdnResourceGroupName, CdnProfileName, CdnEndpointName, contentPaths);
        }
    }
}
