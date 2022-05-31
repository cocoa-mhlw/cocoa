/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Covid19Radar.Common;
using Chino;
using System.Net;

namespace Covid19Radar.Services
{
    public abstract class AbsExposureDetectionBackgroundService : IBackgroundService
    {
        private const string DIAGNOSIS_KEYS_DIR = "diagnosis_keys";

        private readonly IDiagnosisKeyRepository _diagnosisKeyRepository;
        private readonly AbsExposureNotificationApiService _exposureNotificationApiService;
        private readonly ILoggerService _loggerService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly ILocalPathService _localPathService;
        private readonly IDateTimeUtility _dateTimeUtility;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public AbsExposureDetectionBackgroundService(
            IDiagnosisKeyRepository diagnosisKeyRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IServerConfigurationRepository serverConfigurationRepository,
            ILocalPathService localPathService,
            IDateTimeUtility dateTimeUtility
            )
        {
            _diagnosisKeyRepository = diagnosisKeyRepository;
            _exposureNotificationApiService = exposureNotificationApiService;
            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
            _serverConfigurationRepository = serverConfigurationRepository;
            _localPathService = localPathService;
            _dateTimeUtility = dateTimeUtility;
        }

        public abstract void Schedule();

        public virtual async Task ExposureDetectionAsync(CancellationTokenSource cancellationTokenSource = null)
        {
            await _semaphore.WaitAsync();

            _loggerService.StartMethod();

            try
            {
                await InternalExposureDetectionAsync(cancellationTokenSource);
            }
            finally
            {
                _loggerService.EndMethod();

                _semaphore.Release();
            }
        }

        private async Task InternalExposureDetectionAsync(CancellationTokenSource cancellationTokenSource = null)
        {
            bool isEnabled = await _exposureNotificationApiService.IsEnabledAsync();
            if (!isEnabled)
            {
                _loggerService.Info($"EN API is not enabled.");
                return;
            }

            IEnumerable<int> statuseCodes = await _exposureNotificationApiService.GetStatusCodesAsync();

            bool isActivated = statuseCodes.Contains(ExposureNotificationStatus.Code_Android.ACTIVATED)
                | statuseCodes.Contains(ExposureNotificationStatus.Code_iOS.Active);

            if (!isActivated)
            {
                _loggerService.Info($"EN API is not ACTIVATED.");
                return;
            }

            var cancellationToken = cancellationTokenSource?.Token ?? default(CancellationToken);

            await _serverConfigurationRepository.LoadAsync();

            bool canConfirmExposure = true;
            bool isMaxPerDayExposureDetectionAPILimitReached = false;

            foreach (var region in _serverConfigurationRepository.Regions)
            {
                _loggerService.Info($"Region: {region}");

                var diagnosisKeyListProvideServerUrl = _serverConfigurationRepository.GetDiagnosisKeyListProvideServerUrl(region);

                _loggerService.Info($"diagnosisKeyListProvideServerUrl: {diagnosisKeyListProvideServerUrl}");

                List<string> downloadedFileNameList = new List<string>();
                try
                {
                    var tmpDir = PrepareDir(region);

                    var (httpStatus, diagnosisKeyEntryList) = await _diagnosisKeyRepository.GetDiagnosisKeysListAsync(
                        diagnosisKeyListProvideServerUrl,
                        cancellationToken
                        );

                    if (httpStatus != HttpStatusCode.OK)
                    {
                        _loggerService.Info($"URL: {diagnosisKeyListProvideServerUrl}, Response StatusCode: {httpStatus}");
                        canConfirmExposure = false;
                        continue;
                    }

                    var lastProcessTimestamp = await _userDataRepository.GetLastProcessDiagnosisKeyTimestampAsync(region);
                    _loggerService.Info($"Region: {region}, lastProcessTimestamp: {lastProcessTimestamp}");

                    var targetDiagnosisKeyEntryList = FilterDiagnosisKeysAfterLastProcessTimestamp(diagnosisKeyEntryList, lastProcessTimestamp);

                    if (targetDiagnosisKeyEntryList.Count() == 0)
                    {
                        _loggerService.Info($"No new diagnosis-key found on {diagnosisKeyListProvideServerUrl}");
                        continue;
                    }

                    _loggerService.Info($"{targetDiagnosisKeyEntryList.Count()} new keys found.");

                    foreach (var diagnosisKeyEntry in targetDiagnosisKeyEntryList)
                    {
                        string filePath = await _diagnosisKeyRepository.DownloadDiagnosisKeysAsync(diagnosisKeyEntry, tmpDir, cancellationToken);

                        _loggerService.Info($"URL {diagnosisKeyEntry.Url} have been downloaded.");

                        downloadedFileNameList.Add(filePath);
                    }

                    var downloadedFileNames = string.Join("\n", downloadedFileNameList);
                    _loggerService.Debug(downloadedFileNames);

                    await _exposureNotificationApiService.ProvideDiagnosisKeysAsync(
                        downloadedFileNameList,
                        cancellationTokenSource
                        );

                    // Save LastProcessDiagnosisKeyTimestamp after ProvideDiagnosisKeysAsync was succeeded.
                    var latestProcessTimestamp = targetDiagnosisKeyEntryList
                        .Select(diagnosisKeyEntry => diagnosisKeyEntry.Created)
                        .Max();
                    await _userDataRepository.SetLastProcessDiagnosisKeyTimestampAsync(region, latestProcessTimestamp);

                    _userDataRepository.SetLastConfirmedDate(_dateTimeUtility.UtcNow);
                    _userDataRepository.SetCanConfirmExposure(true);
                    _userDataRepository.SetIsMaxPerDayExposureDetectionAPILimitReached(isMaxPerDayExposureDetectionAPILimitReached);
                }
                catch (ENException exception)
                {
                    canConfirmExposure = false;
                    isMaxPerDayExposureDetectionAPILimitReached = CheckMaxPerDayExposureDetectionAPILimitReached(exception);
                    _loggerService.Exception($"ENExcepiton occurred, Code:{exception.Code}, Message:{exception.Message}", exception);
                    throw;
                }
                catch (Exception exception)
                {
                    canConfirmExposure = false;
                    _loggerService.Exception($"Exception occurred: {region}", exception);
                    throw;
                }
                finally
                {
                    RemoveFiles(downloadedFileNameList);
                    _userDataRepository.SetCanConfirmExposure(canConfirmExposure);
                    _userDataRepository.SetIsMaxPerDayExposureDetectionAPILimitReached(isMaxPerDayExposureDetectionAPILimitReached);
                }
            }
        }

        private static IList<DiagnosisKeyEntry> FilterDiagnosisKeysAfterLastProcessTimestamp(
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList,
            long lastProcessTimestamp
            )
        {

#if EN_DEBUG
            // [NOTE] This is trick for inspecting to behavior ExposureNotification API.
            // We're able to reset the diagnosisKeys exposure detecting state by change an app that handling EN API.
            // And so, we have to disable diagnosisKeys filter by lastProcessTimestamp.
            return diagnosisKeyEntryList;
#else
            return diagnosisKeyEntryList
                        .Where(diagnosisKeyEntry => diagnosisKeyEntry.Created > lastProcessTimestamp).ToList();
#endif

        }

        private string PrepareDir(string region)
        {
            var cacheDir = _localPathService.CacheDirectory;

            var baseDir = Path.Combine(cacheDir, DIAGNOSIS_KEYS_DIR);
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            var dir = Path.Combine(baseDir, region);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        private void RemoveFiles(List<string> fileList)
        {
            _loggerService.StartMethod();

            foreach (var file in fileList)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception exception)
                {
                    _loggerService.Exception("Exception occurred", exception);
                }
            }

            _loggerService.EndMethod();
        }

        private bool CheckMaxPerDayExposureDetectionAPILimitReached(ENException ex)
        {
            return ex.Code == ENException.Code_iOS.RateLimited || ex.Code == ENException.Code_Android.FAILED_RATE_LIMITED;
        }
    }
}
