// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public interface IEventLogRepository
    {
        public Task<bool> AddAsync(EventLog eventLog, long maxSize);

        public Task<List<EventLog>> GetLogsAsync(long maxSize);

        public Task<bool> RemoveAsync(EventLog eventLog);

        public Task AddAsync(
            ExposureConfiguration exposureConfiguration,
            string model,
            string enVersionStr,
            IList<DailySummary> dailySummaries,
            IList<ExposureWindow> exposureWindows,
            long maxSize
            );

        public Task AddAsync(
            ExposureConfiguration exposureConfiguration,
            string model,
            string enVersionStr,
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformation,
            long maxSize
            );
    }

    public class EventLogRepository : IEventLogRepository
    {
        private readonly IUserDataRepository _userDataRepository;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly ILoggerService _loggerService;

        private readonly string _basePath;

        public EventLogRepository(
            IUserDataRepository userDataRepository,
            IDateTimeUtility dateTimeUtility,
            ILocalPathService localPathService,
            ILoggerService loggerService
            )
        {
            _userDataRepository = userDataRepository;
            _dateTimeUtility = dateTimeUtility;
            _basePath = localPathService.EventLogDirPath;
            _loggerService = loggerService;
        }

        private string GetFileName(EventLog eventLog)
        {
            var clearText = string.Join(".", eventLog.HasConsent, eventLog.Epoch, eventLog.Type, eventLog.Subtype, eventLog.Content);

            using var sha = SHA256.Create();
            var textBytes = Encoding.UTF8.GetBytes(clearText);
            string fileName = Convert.ToBase64String(sha.ComputeHash(textBytes));

            return fileName;
        }

        public async Task<bool> AddAsync(EventLog eventLog, long maxSize)
        {
            _loggerService.StartMethod();

            try
            {
                string fileName = GetFileName(eventLog);
                string filePath = Path.Combine(_basePath, fileName);

                if (File.Exists(filePath))
                {
                    _loggerService.Info($"{filePath} already exist.");
                    return false;
                }

                var serializedJson = JsonConvert.SerializeObject(eventLog);

                // Check log size.
                long size = Encoding.UTF8.GetByteCount(serializedJson);
                if (size > maxSize)
                {
                    _loggerService.Info($"Log size {size} exceed maxSize( {maxSize} bytes.");
                    return false;
                }

                await File.WriteAllTextAsync(filePath, serializedJson);

                return true;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task<List<EventLog>> GetLogsAsync(long maxSize)
        {
            _loggerService.StartMethod();

            try
            {
                long currentSize = 0;
                var resultList = new List<EventLog>();

                string[] files = Directory.GetFiles(_basePath);
                if (files.Length == 0)
                {
                    _loggerService.Info("No log found.");
                    return resultList;
                }

                List<string> filePathList = files.Select(file => Path.Combine(_basePath, file)).ToList();
                foreach (var path in filePathList)
                {
                    string content = await File.ReadAllTextAsync(path);

                    // Check result size.
                    long size = Encoding.UTF8.GetByteCount(content);
                    long expectSize = currentSize + size;
                    if (expectSize > maxSize)
                    {
                        _loggerService.Info($"Result log size will exceed maxSize( {maxSize} bytes.");
                        continue;
                    }

                    EventLog eventLog = JsonConvert.DeserializeObject<EventLog>(content);
                    resultList.Add(eventLog);
                }

                return resultList;
            }
            finally
            {
                _loggerService.EndMethod();
            }

        }

        public Task<bool> RemoveAsync(EventLog eventLog)
        {
            _loggerService.StartMethod();

            try
            {
                string fileName = GetFileName(eventLog);
                string filePath = Path.Combine(_basePath, fileName);

                if (!File.Exists(filePath))
                {
                    _loggerService.Info($"{filePath} not found.");
                    return Task.FromResult(false);
                }

                File.Delete(filePath);

                _loggerService.Info($"{filePath} is deleted.");

                return Task.FromResult(true);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task AddAsync(
            ExposureConfiguration exposureConfiguration,
            string model,
            string enVersionStr,
            IList<DailySummary> dailySummaries,
            IList<ExposureWindow> exposureWindows,
            long maxSize
            )
        {
            var exposureData = new ExposureData(
                exposureConfiguration,
                dailySummaries,
                exposureWindows
                )
            {
                Device = model,
                EnVersion = enVersionStr
            };

            var eventLog = new EventLog()
            {
                HasConsent = _userDataRepository.IsSendEventLogEnabled(),
                Epoch = _dateTimeUtility.UtcNow.ToUnixEpoch(),
                Type = "ExposureData",
                Subtype = "ExposureWindow",
                Content = exposureData.ToJsonString(),
            };
            await AddAsync(eventLog, maxSize);
        }

        public async Task AddAsync(
            ExposureConfiguration exposureConfiguration,
            string model,
            string enVersionStr,
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformations,
            long maxSize
            )
        {
            var exposureData = new ExposureData(
                exposureConfiguration,
                exposureSummary,
                exposureInformations
                )
            {
                Device = model,
                EnVersion = enVersionStr
            };

            var eventLog = new EventLog()
            {
                HasConsent = _userDataRepository.IsSendEventLogEnabled(),
                Epoch = _dateTimeUtility.UtcNow.ToUnixEpoch(),
                Type = "ExposureData",
                Subtype = "Legacy-v1",
                Content = exposureData.ToJsonString(),
            };
            await AddAsync(eventLog, maxSize);
        }
    }

    public class ExposureData
    {
        private string _device = "unknown_device";

        [JsonProperty("device")]
        public string Device
        {
            get
            {
                return _device;
            }
            set
            {
                string device = value.Replace(" ", "_");
                _device = device;
            }
        }

        [JsonProperty("en_version")]
        public string? EnVersion;

        [JsonProperty("exposure_summary")]
        public readonly ExposureSummary? exposureSummary;

        [JsonProperty("exposure_informations")]
        public readonly IList<ExposureInformation>? exposureInformations;

        [JsonProperty("daily_summaries")]
        public readonly IList<DailySummary>? dailySummaries;

        [JsonProperty("exposure_windows")]
        public readonly IList<ExposureWindow>? exposureWindows;

        [JsonProperty("exposure_configuration")]
        public readonly ExposureConfiguration exposureConfiguration;

        public ExposureData(ExposureConfiguration exposureConfiguration)
            : this(exposureConfiguration, null, null, null, null) { }

        public ExposureData(ExposureConfiguration exposureConfiguration,
            ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
            : this(exposureConfiguration, exposureSummary, exposureInformations, null, null) { }

        public ExposureData(ExposureConfiguration exposureConfiguration,
            IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
            : this(exposureConfiguration, null, null, dailySummaries, exposureWindows) { }

        public ExposureData(ExposureConfiguration exposureConfiguration,
            ExposureSummary? exposureSummary, IList<ExposureInformation>? exposureInformations,
            IList<DailySummary>? dailySummaries, IList<ExposureWindow>? exposureWindows)
        {
            this.exposureConfiguration = exposureConfiguration;
            this.exposureSummary = exposureSummary;
            this.exposureInformations = exposureInformations;
            this.dailySummaries = dailySummaries;
            this.exposureWindows = exposureWindows;
        }

        public string ToJsonString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

}
