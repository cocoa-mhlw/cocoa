// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services
{
    public interface IExposureDataExportService
    {
        Task ExportAsync(string path);
    }

    public class ExposureDataExportService : IExposureDataExportService
    {
        private readonly ILoggerService _loggerService;

        private readonly IExposureDataRepository _exposureDataRepository;

        private readonly IExposureConfigurationRepository _exposureConfigurationRepository;
        private readonly AbsExposureNotificationApiService _exposureNotificationApiService;

        private readonly IEssentialsService _essentialsService;

        public ExposureDataExportService(
            ILoggerService loggerService,
            IExposureDataRepository exposureDataRepository,
            IExposureConfigurationRepository exposureConfigurationRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            IEssentialsService essentialsService
            )
        {
            _loggerService = loggerService;
            _exposureDataRepository = exposureDataRepository;
            _exposureConfigurationRepository = exposureConfigurationRepository;
            _exposureNotificationApiService = exposureNotificationApiService;
            _essentialsService = essentialsService;
        }

        public async Task ExportAsync(string filePath)
        {
            _loggerService.StartMethod();

            try
            {
                long enVersion = await _exposureNotificationApiService.GetVersionAsync();
                List<DailySummary> dailySummaryList = await _exposureDataRepository.GetDailySummariesAsync();
                List<ExposureWindow> exposureWindowList = await _exposureDataRepository.GetExposureWindowsAsync();
                ExposureConfiguration exposureConfiguration = await _exposureConfigurationRepository.GetExposureConfigurationAsync();

                var exposureData = new ExposureData(
                    _essentialsService.Platform,
                    _essentialsService.PlatformVersion,
                    _essentialsService.Model,
                    _essentialsService.DeviceType,
                    _essentialsService.AppVersion,
                    _essentialsService.BuildNumber,
                    enVersion.ToString(),
                    dailySummaryList,
                    exposureWindowList,
                    exposureConfiguration
                    );

                string exposureDataJson = JsonConvert.SerializeObject(exposureData, Formatting.Indented);

                await File.WriteAllTextAsync(filePath, exposureDataJson);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}
