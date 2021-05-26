/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public class TemporaryExposureKeyDeleteBatchService : ITemporaryExposureKeyDeleteBatchService
    {
        public readonly ITemporaryExposureKeyRepository TekRepository;
        public readonly ITemporaryExposureKeyExportRepository TekExportRepository;
        public readonly ITemporaryExposureKeyBlobService BlobService;
        public readonly ILogger<TemporaryExposureKeyDeleteBatchService> Logger;

        public TemporaryExposureKeyDeleteBatchService(
            IConfiguration config,
            ITemporaryExposureKeyRepository tek,
            ITemporaryExposureKeyExportRepository tekExport,
            ITemporaryExposureKeyBlobService blobService,
            ILogger<TemporaryExposureKeyDeleteBatchService> logger)
        {
            Logger = logger;
            Logger.LogInformation($"{nameof(TemporaryExposureKeyDeleteBatchService)} constructor");
            TekRepository = tek;
            TekExportRepository = tekExport;
            BlobService = blobService;
        }

        public async Task RunAsync()
        {
            try
            {
                Logger.LogInformation($"start {nameof(RunAsync)}");
                var deletingItems = await TekExportRepository.GetOutOfTimeKeysAsync();

                // delete blob
                foreach (var item in deletingItems)
                {
                    try
                    {
                        item.Deleted = true;
                        await BlobService.DeleteAsync(item);
                        await TekExportRepository.UpdateAsync(item);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"delete blob Error on {nameof(TemporaryExposureKeyDeleteBatchService)} {item.id}");
                    }
                }

                // delete TemporaryExposureKey
                var tekItems = await TekRepository.GetOutOfTimeKeysAsync();
                // delete blob
                foreach (var item in tekItems)
                {
                    try
                    {
                        await TekRepository.DeleteAsync(item);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"delete blob Error on {nameof(TemporaryExposureKeyDeleteBatchService)} {item.id}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error on {nameof(TemporaryExposureKeyDeleteBatchService)}");
                throw;
            }
        }
    }
}
