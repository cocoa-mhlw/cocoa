﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Extensions;
using Covid19Radar.Api.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

[assembly: FunctionsStartup(typeof(Covid19Radar.Api.Startup))]

namespace Covid19Radar.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddCosmosClient();
            builder.Services.AddSingleton<DataStore.ICosmos, DataStore.Cosmos>();
            builder.Services.AddSingleton<IValidationServerService, ValidationServerService>();
            builder.Services.AddSingleton<IValidationInquiryLogService, ValidationInquiryLogService>();
            builder.Services.AddSingleton<IAuthorizedAppRepository, ConfigAuthorizedAppRepository>();
            builder.Services.AddSingleton<IUserRepository, CosmosUserRepository>();
            builder.Services.AddSingleton<ISequenceRepository, CosmosSequenceRepository>();
            builder.Services.AddSingleton<ITemporaryExposureKeyRepository, CosmosTemporaryExposureKeyRepository>();
            builder.Services.AddSingleton<ITemporaryExposureKeyExportRepository, CosmosTemporaryExposureKeyExportRepository>();
            builder.Services.AddSingleton<IVerificationService, CustomVerificationService>();
            builder.Services.AddSingleton<ICustomVerificationStatusRepository, CustomVerificationStatusRepository>();
            builder.Services.AddSingleton<IInquiryLogBlobService, InquiryLogBlobService>();
            builder.Services.AddSingleton<IDeviceValidationService, DeviceValidationService>();
            builder.Services.AddSingleton<IEventLogRepository, CosmosEventLogRepository>();
            builder.Services.AddSingleton<ITemporaryExposureKeyValidationService, TemporaryExposureKeyValidationService>();
        }
    }
}
