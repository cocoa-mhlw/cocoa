using Covid19Radar.Background.DataStore;
using Covid19Radar.Background.Services;
using Covid19Radar.DataAccess;
using Covid19Radar.DataStore;
using Covid19Radar.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Covid19Radar.Background.Startup))]

namespace Covid19Radar.Background
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddSingleton<ICosmos, Cosmos>();
            builder.Services.AddSingleton<IStoringCosmos, StoringCosmos>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddSingleton<ISequenceRepository, CosmosSequenceRepository>();
            builder.Services.AddSingleton<ITemporaryExposureKeyRepository, CosmosTemporaryExposureKeyRepository>();
            builder.Services.AddSingleton<ITemporaryExposureKeyExportRepository, CosmosTemporaryExposureKeyExportRepository>();
            builder.Services.AddSingleton<ITemporaryExposureKeyExportBatchService, TemporaryExposureKeyExportBatchService>();
            builder.Services.AddSingleton<ITemporaryExposureKeyDeleteBatchService, TemporaryExposureKeyDeleteBatchService>();
            builder.Services.AddSingleton<ITemporaryExposureKeySignatureInfoService, TemporaryExposureKeySignatureInfoService>();
            builder.Services.AddSingleton<ITemporaryExposureKeyBlobService, TemporaryExposureKeyBlobService>();
#if DEBUG
            builder.Services.AddSingleton<ITemporaryExposureKeySignService, TemporaryExposureKeySignServiceDebug>();
#else
            builder.Services.AddSingleton<ITemporaryExposureKeySignService, TemporaryExposureKeySignService>();
#endif

        }
    }
}

