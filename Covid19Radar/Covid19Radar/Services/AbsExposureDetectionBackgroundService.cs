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
using Chino;

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

            var regions = _serverConfigurationRepository.Regions.ToList();
            var subRegions = _serverConfigurationRepository.SubRegions.ToList();

            var exposureConfiguration = await _exposureConfigurationRepository.GetExposureConfigurationAsync();

            foreach (var region in regions)
            {
                List<DiagnosisKeyFile> diagnosisKeyFileList = new List<DiagnosisKeyFile>();

                #region Sub-regoin leval
                try
                {
                    foreach (var subRegion in subRegions)
                    {
                        var files = await DownloadFiles(region, subRegion, cancellationToken);

                        _loggerService.Info($"Sub-region: {subRegion}, {files.Count} files downloaded.");

                        diagnosisKeyFileList.AddRange(files);
                    }

                    _loggerService.Info($"Region: {region}, Sub-region level, {diagnosisKeyFileList.Count} files downloaded.");

                    await DetectExposure(diagnosisKeyFileList, exposureConfiguration, cancellationTokenSource);

                    // Save LastProcessDiagnosisKeyTimestamp after DetectExposure was succeeded.
                    var subRegionGroup = diagnosisKeyFileList.GroupBy(file => file.SubRegion);
                    foreach (var sr in subRegionGroup)
                    {
                        var latestProcessTimestamp = sr
                            .Select(diagnosisKeyFile => diagnosisKeyFile.Created)
                            .Max();
                        await _userDataRepository.SetLastProcessDiagnosisKeyTimestampAsync(region, sr.Key, latestProcessTimestamp);
                    }

                }
                catch (Exception exception)
                {
                    _loggerService.Exception("Exception occurred at Sub-region level.", exception);
                }
                finally
                {
                    RemoveFiles(diagnosisKeyFileList);
                    diagnosisKeyFileList.Clear();
                }
                #endregion

                #region Regoin leval
                var withRegionLevel = !subRegions.Any() || _serverConfigurationRepository.WithRegionLevel;
                if (withRegionLevel)
                {
                    try
                    {
                        diagnosisKeyFileList = await DownloadFiles(region, string.Empty, cancellationToken);

                        _loggerService.Info($"Region {diagnosisKeyFileList.Count} files downloaded.");

                        await DetectExposure(diagnosisKeyFileList, exposureConfiguration, cancellationTokenSource);

                        // Save LastProcessDiagnosisKeyTimestamp after DetectExposure was succeeded.
                        var latestProcessTimestamp = diagnosisKeyFileList
                            .Select(diagnosisKeyFile => diagnosisKeyFile.Created)
                            .Max();
                        await _userDataRepository.SetLastProcessDiagnosisKeyTimestampAsync(region, string.Empty, latestProcessTimestamp);
                    }
                    catch (Exception exception)
                    {
                        _loggerService.Exception("Exception occurred at Region level.", exception);
                    }
                    finally
                    {
                        RemoveFiles(diagnosisKeyFileList);
                        diagnosisKeyFileList.Clear();
                    }
                }
                #endregion
            }
        }

        private async Task DetectExposure(
            List<DiagnosisKeyFile> downloadedFileList,
            ExposureConfiguration exposureConfiguration,
            CancellationTokenSource cancellationTokenSource
            )
        {
            _loggerService.StartMethod();

            try
            {
                _exposureNotificationApiService.ExposureConfiguration = exposureConfiguration;

                _loggerService.Info("Run DetectExposure/ProvideDiagnosisKeysAsync.");

                await _exposureNotificationApiService.ProvideDiagnosisKeysAsync(
                    downloadedFileList.Select(file => file.Path).ToList(),
                    cancellationTokenSource
                    );

                _loggerService.Info("Finish DetectExposure/ProvideDiagnosisKeysAsync.");
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        private async Task<List<DiagnosisKeyFile>> DownloadFiles(
            string region,
            string subRegion,
            CancellationToken cancellationToken
            )
        {
            _loggerService.StartMethod();

            List<DiagnosisKeyFile> downloadedFileList = new List<DiagnosisKeyFile>();

            try
            {
                var diagnosisKeyListProvideServerUrl = _serverConfigurationRepository.GetDiagnosisKeyListProvideServerUrl(region, subRegion);

                _loggerService.Info($"diagnosisKeyListProvideServerUrl: {diagnosisKeyListProvideServerUrl}");

                var tmpDir = PrepareDir(region, subRegion);

                var diagnosisKeyEntryList = await _diagnosisKeyRepository.GetDiagnosisKeysListAsync(diagnosisKeyListProvideServerUrl, cancellationToken);

                var lastProcessTimestamp = await _userDataRepository.GetLastProcessDiagnosisKeyTimestampAsync(region, subRegion);
                var targetDiagnosisKeyEntryList = diagnosisKeyEntryList;

#if DEBUG
                // Do nothing
#else
                targetDiagnosisKeyEntryList = targetDiagnosisKeyEntryList
                    .Where(diagnosisKeyEntry => diagnosisKeyEntry.Created > lastProcessTimestamp).ToList();
#endif

                if (targetDiagnosisKeyEntryList.Count() == 0)
                {
                    _loggerService.Info($"No new diagnosis-key found on {diagnosisKeyListProvideServerUrl}");
                    return downloadedFileList;
                }

                foreach (var diagnosisKeyEntry in targetDiagnosisKeyEntryList)
                {
                    string filePath = await _diagnosisKeyRepository.DownloadDiagnosisKeysAsync(diagnosisKeyEntry, tmpDir, cancellationToken);

                    _loggerService.Debug($"URL {diagnosisKeyEntry.Url} have been downloaded.");

                    downloadedFileList.Add(new DiagnosisKeyFile(region, subRegion, diagnosisKeyEntry.Created, filePath));
                }
            }
            catch (Exception)
            {
                RemoveFiles(downloadedFileList);
                downloadedFileList.Clear();
            }

            var downloadedFilePaths = string.Join("\n", downloadedFileList.Select(file => file.Path));
            _loggerService.Debug(downloadedFilePaths);

            _loggerService.EndMethod();

            return downloadedFileList;
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

        private string PrepareDir(string region, string subRegion)
        {
            var cacheDir = _localPathService.CacheDirectory;

            var dir = Path.Combine(cacheDir, DIAGNOSIS_KEYS_DIR);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            dir = Path.Combine(dir, region);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!string.IsNullOrEmpty(subRegion))
            {
                return dir;
            }

            dir = Path.Combine(dir, subRegion);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }

        private void RemoveFiles(List<DiagnosisKeyFile> fileList)
        {
            foreach (var file in fileList)
            {
                try
                {
                    File.Delete(file.Path);
                }
                catch (Exception exception)
                {
                    _loggerService.Exception("Exception occurred", exception);
                }
            }
        }

        private class DiagnosisKeyFile
        {
            public readonly string Region;
            public readonly string SubRegion;
            public readonly long Created;
            public readonly string Path;

            public DiagnosisKeyFile(
                string region,
                string subRegion,
                long created,
                string path
                )
            {
                Region = region;
                SubRegion = subRegion;
                Created = created;
                Path = path;
            }
        }
    }
}
