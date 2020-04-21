using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Covid19Radar
{
    public class ContactFunction
    {
        private readonly ICosmos Cosmos;
        private readonly ILogger<ContactFunction> Logger;

        public ContactFunction(ICosmos cosmos, ILogger<ContactFunction> logger)
        {
            Cosmos = cosmos;
            Logger = logger;
        }

        [FunctionName("ContactFunction")]
        public async void Run([CosmosDBTrigger(
            databaseName: "EXAMPLE",
            collectionName: "Beacons",
            ConnectionStringSetting = "COSMOS_CONNECTION",
            LeaseCollectionName = "Lease", CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Microsoft.Azure.Documents.Document> input)
        {
            Logger.LogInformation($"{nameof(ContactFunction)} processed a request.");
            foreach (var i in input)
            {
                var d = JsonConvert.DeserializeObject<BeaconModel>(i.ToString());
                Logger.LogInformation($"{nameof(ContactFunction)}  Change feed Major:{d.UserMajor} Minor:{d.UserMinor}");
                var p = await QueryPair(d);
                if (p == null) continue;
                BeaconModel b1, b2;
                IUserMajorMinorExtension.SetDecideLeftRight(d, p, out b1, out b2);
                await Upsert(b1, b2);
            }
        }

        private async Task<BeaconModel> QueryPair(BeaconModel input)
        {
            // pair
            var queryPair = new QueryDefinition
                ($"SELECT * FROM {Cosmos.ContainerNameBeacon} b WHERE b.UserMajor = @UserMajor and b.UserMinor = @UserMinor and b.KeyTime = @KeyTime")
                .WithParameter("@UserMajor", input.Major)
                .WithParameter("@UserMinor", input.Minor)
                .WithParameter("@KeyTime", input.KeyTime);
            var option = new QueryRequestOptions();
            option.PartitionKey = new PartitionKey($"{input.Major}.{input.Minor}");
            try
            {
                var pair = await Cosmos.Beacon.GetItemQueryIterator<BeaconModel>(queryPair, null, option).ReadNextAsync();
                return pair.FirstOrDefault();
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.LogInformation($"{nameof(ContactFunction)} Not Found Major:{input.Major} Minor:{input.Major}");
                    return null;
                }
                Logger.LogError(ex, $"{nameof(ContactFunction)} Throw from QueryPair Major:{input.Major} Minor:{input.Major}");
            }
            return null;
        }

        private async Task Upsert(BeaconModel b1, BeaconModel b2)
        {
            var item = new ContactModel();
            var pk = $"{b1.Major}.{b1.Minor}-{b2.Major}.{b2.Minor}";
            item.id = $"{b1.Major}.{b1.Minor}-{b1.KeyTime}-{b2.Major}.{b2.Minor}";
            item.KeyTime = b1.KeyTime;
            item.PartitionKey = pk;
            item.Beacon1 = b1;
            item.Beacon2 = b2;
            try
            {
                var result = await Cosmos.Contact.UpsertItemAsync(item, new PartitionKey(pk));
                Logger.LogInformation($"{nameof(ContactFunction)} Complete Upsert id:{item.id}");
            }
            catch (CosmosException ex)
            {
                Logger.LogError(ex, $"{nameof(ContactFunction)} Throw from Upsert id:{item.id}");
            }

        }
    }
}
