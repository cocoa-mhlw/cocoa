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

namespace Covid19Radar.Services
{
    public abstract class AbsExposureDetectionBackgroundService : IBackgroundService
    {
        private const string DIAGNOSIS_KEYS_DIR = "diagnosis_keys";

        private readonly IDiagnosisKeyRepository _diagnosisKeyRepository;
        private readonly AbsExposureNotificationApiService _exposureNotificationApiService;
        private readonly IExposureConfigurationRepository _exposureConfigurationRepository;
        private readonly ILoggerService _loggerService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly ILocalPathService _localPathService;

        public AbsExposureDetectionBackgroundService(
            IDiagnosisKeyRepository diagnosisKeyRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IServerConfigurationRepository serverConfigurationRepository,
            ILocalPathService localPathService
            )
        {
            _diagnosisKeyRepository = diagnosisKeyRepository;
            _exposureNotificationApiService = exposureNotificationApiService;
            _exposureConfigurationRepository = exposureConfigurationRepository;
            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
            _serverConfigurationRepository = serverConfigurationRepository;
            _localPathService = localPathService;
        }

        public abstract void Schedule();

        public virtual async Task ExposureDetectionAsync(CancellationTokenSource cancellationTokenSource = null)
        {
            var cancellationToken = cancellationTokenSource?.Token ?? default(CancellationToken);

            await _serverConfigurationRepository.LoadAsync();

            foreach (var region in _serverConfigurationRepository.Regions)
            {
                var diagnosisKeyListProvideServerUrl = _serverConfigurationRepository.GetDiagnosisKeyListProvideServerUrl(region);

                List<string> downloadedFileNameList = new List<string>();
                try
                {
                    var tmpDir = PrepareDir(region);

                    var exposureConfiguration = await _exposureConfigurationRepository.GetExposureConfigurationAsync();
                    var diagnosisKeyEntryList = await _diagnosisKeyRepository.GetDiagnosisKeysListAsync(diagnosisKeyListProvideServerUrl, cancellationToken);

                    var lastProcessTimestamp = await _userDataRepository.GetLastProcessDiagnosisKeyTimestampAsync(region);
                    var targetDiagnosisKeyEntryList = FilterDiagnosisKeysAfterLastProcessTimestamp(diagnosisKeyEntryList, lastProcessTimestamp);

                    if (targetDiagnosisKeyEntryList.Count() == 0)
                    {
                        _loggerService.Info($"No new diagnosis-key found on {diagnosisKeyListProvideServerUrl}");
                        continue;
                    }

                    foreach (var diagnosisKeyEntry in targetDiagnosisKeyEntryList)
                    {
                        string filePath = await _diagnosisKeyRepository.DownloadDiagnosisKeysAsync(diagnosisKeyEntry, tmpDir, cancellationToken);

                        _loggerService.Debug($"URL {diagnosisKeyEntry.Url} have been downloaded.");

                        downloadedFileNameList.Add(filePath);
                    }

                    var downloadedFileNames = string.Join("\n", downloadedFileNameList);
                    _loggerService.Debug(downloadedFileNames);

                    _exposureNotificationApiService.ExposureConfiguration = exposureConfiguration;
                    await _exposureNotificationApiService.ProvideDiagnosisKeysAsync(
                        downloadedFileNameList,
                        exposureConfiguration,
                        cancellationTokenSource
                        );

                    // Save LastProcessDiagnosisKeyTimestamp after ProvideDiagnosisKeysAsync was succeeded.
                    var latestProcessTimestamp = targetDiagnosisKeyEntryList
                        .Select(diagnosisKeyEntry => diagnosisKeyEntry.Created)
                        .Max();
                    await _userDataRepository.SetLastProcessDiagnosisKeyTimestampAsync(region, latestProcessTimestamp);

                }
                finally
                {
                    RemoveFiles(downloadedFileNameList);
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
        }
    }
}
