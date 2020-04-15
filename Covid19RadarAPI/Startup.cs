using System;
using Covid19Radar.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.AspNetCore.Http;

[assembly: FunctionsStartup(typeof(Covid19Radar.Startup))]

namespace Covid19Radar
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<DataStore.ICosmos, DataStore.Cosmos>();
            builder.Services.AddSingleton<Services.INotificationService, Services.NotificationService>();
            builder.Services.AddSingleton<IOtpGenerator, OtpGenerator>();
            builder.Services.AddSingleton<IOtpService, OtpService>();
            builder.Services.AddSingleton<ISmsSender, SmsSender>();
        }
    }
}

