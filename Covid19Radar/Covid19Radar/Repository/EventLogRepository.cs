// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public interface IEventLogRepository
    {
        public Task<List<EventLog>> GetLogsAsync(
            long maxSize = AppConstants.EventLogMaxRequestSizeInBytes
            );

        public Task<bool> RemoveAsync(EventLog eventLog);

        public Task AddEventNotifiedAsync(
            long maxSize = AppConstants.EventLogMaxRequestSizeInBytes
            );
    }

    public class EventLogRepository : IEventLogRepository
    {
        private const string LOG_EXTENSION = ".log";

        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly ILoggerService _loggerService;

        private readonly string _basePath;

        public EventLogRepository(
            ISendEventLogStateRepository sendEventLogStateRepository,
            IDateTimeUtility dateTimeUtility,
            ILocalPathService localPathService,
            ILoggerService loggerService
            )
        {
            _sendEventLogStateRepository = sendEventLogStateRepository;
            _dateTimeUtility = dateTimeUtility;
            _basePath = localPathService.EventLogDirPath;
            _loggerService = loggerService;
        }

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private string GetFileName(EventLog eventLog)
        {
            var clearText = string.Join(".", eventLog.Epoch, eventLog.Type, eventLog.Subtype, eventLog.Content);

            using var sha = SHA256.Create();
            var textBytes = Encoding.UTF8.GetBytes(clearText);
            string hash = Convert.ToBase64String(sha.ComputeHash(textBytes));

            return $"{hash}{LOG_EXTENSION}";
        }

        private async Task<bool> AddAsyncInternal(EventLog eventLog, long maxSize)
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
                    _loggerService.Info($"Log size {size} exceed maxSize {maxSize} bytes.");
                    return false;
                }

                string directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                    _loggerService.Info($"Directory created. directoryName:{directoryName}");
                }

                await File.WriteAllTextAsync(filePath, serializedJson);

                return true;
            }
            catch (Exception ex)
            {
                _loggerService.Exception("Write event log failure.", ex);
            }
            finally
            {
                _loggerService.EndMethod();
            }
            return false;
        }

        public async Task<List<EventLog>> GetLogsAsync(long maxSize = AppConstants.EventLogMaxRequestSizeInBytes)
        {
            _loggerService.StartMethod();

            await _semaphore.WaitAsync();

            try
            {
                return await GetLogsAsyncInternal(maxSize);
            }
            finally
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        private async Task<List<EventLog>> GetLogsAsyncInternal(long maxSize)
        {
            _loggerService.StartMethod();

            try
            {
                long currentSize = 0;
                var resultList = new List<EventLog>();

                string[] files = Directory.GetFiles(_basePath)
                    .Where(file => file.EndsWith(LOG_EXTENSION))
                    .ToArray();
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
                        _loggerService.Info($"Log {path} size will exceed maxSize( {maxSize} bytes.");
                        continue;
                    }

                    try
                    {
                        EventLog eventLog = JsonConvert.DeserializeObject<EventLog>(content);
                        resultList.Add(eventLog);
                    }
                    catch (JsonReaderException exception)
                    {
                        _loggerService.Exception($"Serialize failed {path} will be removed.", exception);
                        File.Delete(path);
                    }
                }

                return resultList;
            }
            finally
            {
                _loggerService.EndMethod();
            }

        }

        public async Task<bool> RemoveAsync(EventLog eventLog)
        {
            _loggerService.StartMethod();

            await _semaphore.WaitAsync();

            try
            {
                return RemoveAsyncInternal(eventLog);
            }
            finally
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        private bool RemoveAsyncInternal(EventLog eventLog)
        {
            _loggerService.StartMethod();

            try
            {
                string fileName = GetFileName(eventLog);
                string filePath = Path.Combine(_basePath, fileName);

                if (!File.Exists(filePath))
                {
                    _loggerService.Info($"{filePath} not found.");
                    return false;
                }

                File.Delete(filePath);

                _loggerService.Info($"{filePath} is deleted.");

                return true;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task AddEventNotifiedAsync(long maxSize = AppConstants.EventLogMaxRequestSizeInBytes)
        {
            _loggerService.StartMethod();

            await _semaphore.WaitAsync();

            try
            {
                await AddEventNotifiedAsyncInternal(maxSize);
            }
            finally
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        private async Task AddEventNotifiedAsyncInternal(long maxSize)
        {
            bool hasConsent = _sendEventLogStateRepository.GetSendEventLogState(
                EventType.ExposureNotified
                ) == SendEventLogState.Enable;

            var content = new EventContentExposureNotified()
            {
                NotifiedTimeInMillis = _dateTimeUtility.UtcNow.Ticks
            };

            var eventLog = new EventLog()
            {
                HasConsent = hasConsent,
                Epoch = _dateTimeUtility.UtcNow.ToUnixEpoch(),
                Type = EventType.ExposureNotified.Type,
                Subtype = EventType.ExposureNotified.SubType,
                Content = content.ToJsonString(),
            };
            await AddAsyncInternal(eventLog, maxSize);
        }
    }

    public class EventContentExposureNotified
    {
        [JsonProperty("notified_time_in_millis")]
        public long NotifiedTimeInMillis { get; set; }

        public string ToJsonString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}