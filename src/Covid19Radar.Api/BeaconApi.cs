using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Covid19Radar.Models;
using Covid19Radar.DataStore;
using Microsoft.Azure.Cosmos;
using Covid19Radar.DataAccess;

#nullable enable

namespace Covid19Radar.Api
{
    public class BeaconApi
    {
        private readonly IBeaconRepository BeaconRepository;
        private readonly IUserRepository UserRepository;
        private ILogger<BeaconApi> Logger;

        public BeaconApi(IBeaconRepository beaconRepository, IUserRepository userRepository, ILogger<BeaconApi> logger)
        {
            BeaconRepository = beaconRepository;
            UserRepository = userRepository;
            Logger = logger;
        }

        [FunctionName(nameof(BeaconApi))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Beacon")] HttpRequest req)
        {
            Logger.LogInformation($"{nameof(BeaconApi)} processed a request.");

            switch (req.Method)
            {
                case "POST":
                    return await Post(req);
            }
            AddBadRequest(req);
            return new BadRequestObjectResult("Not Supported");
        }

        private async Task<IActionResult> Post(HttpRequest req)
        {
            // convert Postdata to BeaconDataModel
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var param = JsonConvert.DeserializeObject<BeaconParameter>(requestBody);

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

            // save to DB
            return await Add(param);
        }

        private async Task<IActionResult?> Query(HttpRequest req, IUser user)
        {
            try
            {
                if (await UserRepository.Exists(user.GetId()))
                {
                    return null;
                }
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

        private async Task<IActionResult> Add(BeaconParameter param)
        {
            var pk = $"{param.UserMajor}.{param.UserMinor}";
            var data = new BeaconModel();
            data.id = $"{param.UserUuid}.{param.Id}";
            data.UserUuid = param.UserUuid;
            data.UserMajor = param.UserMajor;
            data.UserMinor = param.UserMinor;
            data.BeaconUuid = param.BeaconUuid;
            data.Count = param.Count;
            data.Distance = param.Distance;
            data.ElaspedTime = param.ElaspedTime;
            data.FirstDetectTime = param.FirstDetectTime;
            data.LastDetectTime = param.LastDetectTime;
            data.Major = param.Major;
            data.Minor = param.Minor;
            data.Rssi = param.Rssi;
            data.TXPower = param.TXPower;
            data.KeyTime = param.KeyTime;
            data.TimeStamp = DateTime.UtcNow;
            data.PartitionKey = pk;
            await BeaconRepository.Upsert(data);
            return new StatusCodeResult(201);
        }

        private void AddBadRequest(HttpRequest req)
        {
            // add deny list
        }
    }
}
