/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services.Migration
{
    public interface ISequentialVersionMigrationService
    {
        public Task SetupAsync() => Task.CompletedTask;
        public Task Initialize_1_0_0_Async() => Task.CompletedTask;
        public Task MigrateTo_1_2_2_Async() => Task.CompletedTask;
        public Task MigrateTo_1_2_3_Async() => Task.CompletedTask;
    }

    public abstract class IVersionMigrationService
    {
        public abstract Task MigrateAsync();
    }

    public class VersionMigrationService : IVersionMigrationService, ISequentialVersionMigrationService
    {
        protected const string DEFAULT_VERSION = "1.0.0";

        private static readonly Version VERSION_1_0_0 = new Version("1.0.0");
        private static readonly Version VERSION_1_2_2 = new Version("1.2.2");
        private static readonly Version VERSION_1_2_3 = new Version("1.2.3");

        private readonly ISequentialVersionMigrationService _platformVersionMigrationService;

        private readonly IApplicationPropertyService _applicationPropertyService;
        private readonly IPreferencesService _preferencesService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly IEssentialsService _essentialsService;
        private readonly ILoggerService _loggerService;

        public VersionMigrationService(
            ISequentialVersionMigrationService platformVersionMigrationService,
            IApplicationPropertyService applicationPropertyService,
            IPreferencesService preferencesService,
            ISecureStorageService secureStorageService,
            IEssentialsService essentialsService,
            ILoggerService loggerService)
        {
            _platformVersionMigrationService = platformVersionMigrationService;
            _applicationPropertyService = applicationPropertyService;
            _preferencesService = preferencesService;
            _secureStorageService = secureStorageService;
            _essentialsService = essentialsService;
            _loggerService = loggerService;
        }

        private Version CurrentAppVersion
        {
            get
            {
                _loggerService.StartMethod();

                string appVersion = _essentialsService.AppVersion;
                _loggerService.Debug($"AppVersion: {appVersion}");

                _loggerService.EndMethod();
                return new Version(appVersion);
            }
        }

        private Version PreferenceVersion
        {
            get
            {
                _loggerService.StartMethod();

                if (!_preferencesService.ContainsKey(PreferenceKey.AppVersion))
                {
                    _loggerService.Debug($"appVersion entry is not found in Preferences.");
                    return null;
                }
                var appVersion = _preferencesService.GetValue<string>(PreferenceKey.AppVersion, DEFAULT_VERSION);
                _loggerService.Info($"Current Preference Version: {appVersion}");

                _loggerService.EndMethod();

                return new Version(appVersion);
            }
        }

        private Task<bool> DetectDowngradeAsync()
        {
            var fromVersion = PreferenceVersion;
            fromVersion = fromVersion is null ? GuessVersion() : fromVersion;

            return Task.FromResult((fromVersion.CompareTo(CurrentAppVersion) > 0));
        }

        private void UpdateAppVersionPreference(Version version)
            => _preferencesService.SetValue(PreferenceKey.AppVersion, version.ToString());

        private readonly SemaphoreSlim _semaphoreForMigrate = new SemaphoreSlim(1, 1);

        public async override Task MigrateAsync()
        {
            try
            {
                await _semaphoreForMigrate.WaitAsync();
                var fromVersion = PreferenceVersion;
                await MigrateAsync(fromVersion);
            }
            finally
            {
                _semaphoreForMigrate.Release();
            }
        }

        private async Task ClearAllDataAsync()
        {
            _loggerService.StartMethod();

            const string applicationPropertyUserDataKey = "UserData";
            const string applicationPropertyTermsOfServiceLastUpdateDateKey = "TermsOfServiceLastUpdateDateTime";
            const string applicationPropertyPrivacyPolicyLastUpdateDateKey = "PrivacyPolicyLastUpdateDateTime";

            // Remove all ApplicationProperties
            await _applicationPropertyService.Remove(applicationPropertyUserDataKey);
            await _applicationPropertyService.Remove(applicationPropertyTermsOfServiceLastUpdateDateKey);
            await _applicationPropertyService.Remove(applicationPropertyPrivacyPolicyLastUpdateDateKey);
            await _applicationPropertyService.Remove(PreferenceKey.ExposureNotificationConfiguration);

            // Remove all Preferences
            _preferencesService.RemoveValue(PreferenceKey.AppVersion);
            _preferencesService.RemoveValue(PreferenceKey.StartDateTime);
            _preferencesService.RemoveValue(PreferenceKey.TermsOfServiceLastUpdateDateTime);
            _preferencesService.RemoveValue(PreferenceKey.PrivacyPolicyLastUpdateDateTime);
            _preferencesService.RemoveValue(PreferenceKey.ExposureNotificationConfiguration);
            _preferencesService.RemoveValue(PreferenceKey.LastProcessTekTimestamp);

            // Do not remove Exposure informations from SecureStorage
            //_secureStorageService.RemoveValue(PreferenceKey.ExposureSummary);
            //_secureStorageService.RemoveValue(PreferenceKey.ExposureInformation);

            _loggerService.EndMethod();
        }

        private Version GuessVersion()
        {
            if (_preferencesService.ContainsKey(PreferenceKey.StartDateTime))
            {
                return VERSION_1_2_2;
            }
            return VERSION_1_0_0;
        }

        private async Task MigrateAsync(Version? fromVersion)
        {
            _loggerService.StartMethod();

            if (await DetectDowngradeAsync())
            {
                await ClearAllDataAsync();
            }

            if (fromVersion is null)
            {
                fromVersion = GuessVersion();
                if (fromVersion.CompareTo(VERSION_1_0_0) == 0)
                {
                    await Initialize_1_0_0_Async();
                    UpdateAppVersionPreference(fromVersion);
                }
            }

            var currentAppVersion = CurrentAppVersion;

            if (fromVersion.CompareTo(currentAppVersion) == 0)
            {
                _loggerService.Debug($"fromVersion: {fromVersion} == currentAppVersion: {currentAppVersion}");
                _loggerService.EndMethod();
                return;
            }

            await SetupAsync();

            if (fromVersion.CompareTo(VERSION_1_2_2) < 0)
            {
                await MigrateTo_1_2_2_Async();
            }

            if (fromVersion.CompareTo(VERSION_1_2_3) < 0)
            {
                await MigrateTo_1_2_3_Async();
            }

            UpdateAppVersionPreference(currentAppVersion);

            _loggerService.EndMethod();
        }

        public Task SetupAsync()
        {
            // do nothing

            return Task.CompletedTask;
        }

        public async Task Initialize_1_0_0_Async()
        {
            _loggerService.StartMethod();

            await new Initializer_1_0_0(
                _applicationPropertyService,
                _loggerService
                ).MigrateAsync();

            _loggerService.EndMethod();
        }

        public async Task MigrateTo_1_2_2_Async()
        {
            _loggerService.StartMethod();

            await new Migrator_1_2_2(
                _applicationPropertyService,
                _preferencesService,
                _secureStorageService,
                _loggerService
                ).MigrateAsync();

            await _platformVersionMigrationService.MigrateTo_1_2_2_Async();

            UpdateAppVersionPreference(VERSION_1_2_2);

            _loggerService.EndMethod();
        }

        public async Task MigrateTo_1_2_3_Async()
        {
            _loggerService.StartMethod();

            await _platformVersionMigrationService.MigrateTo_1_2_2_Async();

            UpdateAppVersionPreference(VERSION_1_2_3);

            _loggerService.EndMethod();
        }
    }
}
