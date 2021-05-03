﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Extensions;
using Covid19Radar.Background.Services;
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
            builder.Services.AddCosmosClient();
            builder.Services.AddSingleton<ICosmos, Cosmos>();
            builder.Services.AddSingleton<ISequenceRepository, CosmosSequenceRepository>();
            builder.Services.AddSingleton<ITemporaryExposureKeyRepository, CosmosTemporaryExposureKeyRepository>();
            builder.Services.AddSingleton<ITemporaryExposureKeyExportRepository, CosmosTemporaryExposureKeyExportRepository>();
            builder.Services.AddSingleton<ITemporaryExposureKeyExportBatchService, TemporaryExposureKeyExportBatchService>();
            builder.Services.AddSingleton<ITemporaryExposureKeyDeleteBatchService, TemporaryExposureKeyDeleteBatchService>();
            builder.Services.AddSingleton<ITemporaryExposureKeySignatureInfoService, TemporaryExposureKeySignatureInfoService>();
            builder.Services.AddSingleton<ITemporaryExposureKeyBlobService, TemporaryExposureKeyBlobService>();
            builder.Services.AddSingleton<ITemporaryExposureKeySignService, TemporaryExposureKeySignService>();

        }
    }
}

