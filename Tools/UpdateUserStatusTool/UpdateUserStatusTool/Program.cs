using System;
using System.Threading.Tasks;
using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;

namespace UpdateUserStatusTool
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                // Pre-preparation
                var config = new Moq.Mock<Microsoft.Extensions.Configuration.IConfiguration>();
                var setting = System.Configuration.ConfigurationManager.AppSettings;
                string userStatus;
                string major;
                string minor;

                // Validate Arguments
                switch (args.Length)
                {
                    case 3:
                        config.Setup(_ => _.GetSection("COSMOS_ENDPOINT_URI").Value).Returns(() => setting["COSMOS_ENDPOINT_URI"]);
                        config.Setup(_ => _.GetSection("COSMOS_PRIMARY_KEY").Value).Returns(() => setting["COSMOS_PRIMARY_KEY"]);
                        config.Setup(_ => _.GetSection("COSMOS_DATABASE_ID").Value).Returns(() => setting["COSMOS_DATABASE_ID"]);
                        userStatus = args[0];
                        major = args[1];
                        minor = args[2];
                        break;
                    case 6:
                        config.Setup(_ => _.GetSection("COSMOS_ENDPOINT_URI").Value).Returns(() => args[0]);
                        config.Setup(_ => _.GetSection("COSMOS_PRIMARY_KEY").Value).Returns(() => args[1]);
                        config.Setup(_ => _.GetSection("COSMOS_DATABASE_ID").Value).Returns(() => args[2]);
                        userStatus = args[3];
                        major = args[4];
                        minor = args[5];
                        break;
                    default:
                        Console.Error.WriteLine("ERROR Arguments do not match.");
                        WriteHelp();
                        return -1;
                }

                // validate UserStatus
                Covid19Radar.Common.UserStatus status;
                if (!Enum.TryParse<Covid19Radar.Common.UserStatus>(userStatus, out status))
                {
                    Console.Error.WriteLine("ERROR UserStatus miss match.");
                    WriteHelp();
                    return -1;
                }

                config.Setup(_ => _.GetSection("COSMOS_AUTO_GENERATE").Value).Returns(() => "false");
                config.Setup(_ => _.GetSection("COSMOS_BEACON_STORE").Value).Returns(() => "Beacons");
                var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<ICosmos>>();
                var cosmos = new Cosmos(config.Object, logger.Object);

                // query
                var q = new QueryDefinition("SELECT * FROM c WHERE c.Major = @Major and c.Minor = @Minor")
                    .WithParameter("@Major", major)
                    .WithParameter("@Minor", minor);
                var opt = new QueryRequestOptions();
                var iterator = cosmos.User.GetItemQueryIterator<UserModel>(q, null, opt);
                while (iterator.HasMoreResults)
                {
                    var result = await iterator.ReadNextAsync();
                    Console.WriteLine($"Query StatusCode:{result.StatusCode} Charge:{result.RequestCharge}");
                    foreach (var user in result.Resource)
                    {
                        user.SetStatus(status);
                        var itemOption = new ItemRequestOptions();
                        itemOption.IfMatchEtag = user._etag;
                        var replaceResult = await cosmos.User.ReplaceItemAsync<UserModel>(user, user.id, null, itemOption);
                        Console.WriteLine($"ReplaceItem StatusCode:{replaceResult.StatusCode} Charge:{replaceResult.RequestCharge}");
                    }
                }

                Console.WriteLine("Update Completed!!");

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                return -1;
            }
        }

        static void WriteHelp()
        {
            var pgname = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            Console.WriteLine($"Syntax");
            Console.WriteLine($" $ {pgname} CosmosUri CosmosDatabase CosmosPrimaryKey UserStatus Major Minor");
            Console.WriteLine($" $ {pgname} UserStatus Major Minor");
            Console.WriteLine($"Valid UserStatus list");
            Console.Write($"{ string.Join(", ", Enum.GetNames(typeof(Covid19Radar.Common.UserStatus)))} ");
            Console.WriteLine($"");
        }

    }
}
