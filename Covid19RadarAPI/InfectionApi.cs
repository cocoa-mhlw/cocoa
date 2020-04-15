using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
using Covid19Radar.Models;
using Covid19Radar.DataStore;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Covid19Radar
{
    public class InfectionApi
    {
        private readonly ICosmos Cosmos;
        private readonly ILogger<InfectionApi> Logger;
        public InfectionApi(ICosmos cosmos, ILogger<InfectionApi> logger)
        {
            Cosmos = cosmos;
            Logger = logger;
        }

        [FunctionName("Infection")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Infection")] HttpRequest req)
        {
            Logger.LogInformation($"{nameof(InfectionApi)} processed a request.");

            switch (req.Method)
            {
                case "POST":
                    return await Post(req);
            }

            return new BadRequestObjectResult("Not Supported");
        }

        private async Task<IActionResult> Post(HttpRequest req)
        {
            // convert Postdata to BeaconDataModel
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var param = JsonConvert.DeserializeObject<InfectionParameter>(requestBody);

            // validation
            if (string.IsNullOrWhiteSpace(param.UserUuid)
                || string.IsNullOrWhiteSpace(param.Major)
                || string.IsNullOrWhiteSpace(param.Minor))
            {
                AddBadRequest(req);
                return new BadRequestObjectResult("");
            }
            var queryResult = await Query(req, param);
            if (queryResult != null)
            {
                return queryResult;
            }

            // query
            return await QueryMatchContact(req, param);
        }

        private async Task<IActionResult> Query(HttpRequest req, IUser user)
        {
            try
            {
                var itemResult = await Cosmos.User.ReadItemAsync<UserResultModel>(user.GetId(), PartitionKey.None);
                if (itemResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return null;
                }
                // TODO: validation OneTimePassword
            }
            catch (CosmosException ex)
            {
                // 429-TooManyRequests
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return new StatusCodeResult(503);
                }
            }
            AddBadRequest(req);
            return new BadRequestObjectResult("");
        }

        private async Task<IActionResult> QueryMatchContact(HttpRequest req, IUser user)
        {
            try
            {
                // User
                var queryUser = new QueryDefinition("SELECT * FROM Beacon b WHERE b.UserMajor = @Major and b.UserMinor = @Minor");
                queryUser.WithParameter("Major", user.Major);
                queryUser.WithParameter("Minor", user.Minor);
                var rUser = await Cosmos.Beacon.GetItemQueryIterator<BeaconModel>(queryUser).ReadNextAsync();
                // Other
                var queryOther = new QueryDefinition("SELECT * FROM Beacon b WHERE b.Major = @Major and b.Minor = @Minor");
                queryOther.WithParameter("Major", user.Major);
                queryOther.WithParameter("Minor", user.Minor);
                var rOther = await Cosmos.Beacon.GetItemQueryIterator<BeaconModel>(queryOther).ReadNextAsync();
                // validation
                foreach(var u in rUser.Resource)
                {
                    foreach(var o in rOther.Resource)
                    {
                        if (u.Major == o.UserMajor && u.Minor == o.UserMinor)
                        {
                            // target UserUuid
                            var id = o.GetId();
                            await updateUserStatus(id, Common.UserStatus.Contactd);
                        }
                    }
                }
                await updateUserStatus(user.GetId(), Common.UserStatus.Infection);
                return new OkResult();
            }
            catch (CosmosException ex)
            {
                // 429-TooManyRequests
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return new StatusCodeResult(503);
                }
            }
            AddBadRequest(req);
            return new BadRequestObjectResult("");
        }

        private async Task updateUserStatus(string id, Common.UserStatus status)
        {
            for (var i = 0; i < 100; i++)
            {
                var result = await Cosmos.User.ReadItemAsync<UserModel>(id, PartitionKey.None);
                var model = result.Resource;
                model.SetStatus(status);
                var option = new ItemRequestOptions();
                option.IfMatchEtag = result.ETag;
                option.ConsistencyLevel = ConsistencyLevel.Session;
                try
                {
                    var resultReplace = await Cosmos.Sequence.ReplaceItemAsync(model, id, null, option);
                    return;
                }
                catch (CosmosException ex)
                {
                    Logger.LogInformation(ex, $"GetNumber Retry {i}");
                    continue;
                }
            }
            Logger.LogWarning("GetNumber is over retry count.");
        }

        private void AddBadRequest(HttpRequest req)
        {
            // add deny list
        }
    }
}
