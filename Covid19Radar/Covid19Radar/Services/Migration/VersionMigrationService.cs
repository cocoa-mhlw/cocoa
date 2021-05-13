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
    public interface IVersionMigration
    {
        public Task Initialize100Async();
        public Task MigrateTo122Async();
        public Task MigrateTo123Async();
    }

    public abstract class IVersionMigrationService
    {
        public abstract Task MigrateAsync();
    }

    public class VersionMigrationService : IVersionMigrationService, IVersionMigration
    {
        protected const string DEFAULT_VERSION = "1.0.0";

        private static readonly Version VERSION_1_2_2 = new Version("1.2.2");
        private static readonly Version VERSION_1_2_3 = new Version("1.2.3");

        private readonly IVersionMigration _platformVersionMigrationService;

        private readonly IApplicationPropertyService _applicationPropertyService;
        private readonly IPreferencesService _preferencesService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly IEssentialsService _essentialsService;
        private readonly ILoggerService _loggerService;

        public VersionMigrationService(
            IVersionMigration platformVersionMigrationService,
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

        private Version GetAppVersion()
        {
            _loggerService.StartMethod();

            string appVersion = _essentialsService.AppVersion;
            _loggerService.Debug($"AppVersion: {appVersion}");

            _loggerService.EndMethod();
            return new Version(appVersion);
        }

        private Version GetAppVersionFromPreference()
        {
            _loggerService.StartMethod();

            if (!_preferencesService.ContainsKey(PreferenceKey.AppVersion))
            {
                _loggerService.Debug($"appVersion entry is not found in Preferences.");
                return null;
            }
            var appVersion = _preferencesService.GetValue<string>(PreferenceKey.AppVersion, DEFAULT_VERSION);
            _loggerService.Debug($"Current Preference AppVersion: {appVersion}");

            _loggerService.EndMethod();

            return new Version(appVersion);
        }

        private void UpdateAppVersionPreference(Version version)
            => _preferencesService.SetValue(PreferenceKey.AppVersion, version.ToString());

        private readonly SemaphoreSlim _semaphoreForMigrate = new SemaphoreSlim(1, 1);

        public async override Task MigrateAsync()
        {
            try
            {
                await _semaphoreForMigrate.WaitAsync();
                var fromVersion = GetAppVersionFromPreference();
                await MigrateAsync(fromVersion);
            }
            finally
            {
                _semaphoreForMigrate.Release();
            }
        }

        private async Task MigrateAsync(Version? fromVersion)
        {
            _loggerService.StartMethod();

            var currentAppVersion = GetAppVersion();

            if (fromVersion is null)
            {
                if (!_preferencesService.ContainsKey(PreferenceKey.StartDateTime))
                {
                    await Initialize100Async();
                    await _platformVersionMigrationService.Initialize100Async();
                }
                fromVersion = new Version(DEFAULT_VERSION);
                UpdateAppVersionPreference(fromVersion);
            }

            if (fromVersion.CompareTo(currentAppVersion) == 0)
            {
                _loggerService.Debug($"fromVersion: {fromVersion} == currentAppVersion: {currentAppVersion}");
                _loggerService.EndMethod();
                return;
            }

            if (fromVersion.CompareTo(VERSION_1_2_2) < 0)
            {
                await MigrateTo122Async();
                await _platformVersionMigrationService.MigrateTo122Async();
                UpdateAppVersionPreference(VERSION_1_2_2);
            }

            if (fromVersion.CompareTo(VERSION_1_2_3) < 0)
            {
                await MigrateTo123Async();
                await _platformVersionMigrationService.MigrateTo123Async();
                UpdateAppVersionPreference(VERSION_1_2_3);
            }

            UpdateAppVersionPreference(currentAppVersion);

            _loggerService.EndMethod();
        }

        public async Task Initialize100Async()
        {
            _loggerService.StartMethod();

            await new Initializer100(
                _applicationPropertyService,
                _loggerService
                ).MigrateAsync();

            _loggerService.EndMethod();
        }

        public async Task MigrateTo122Async()
        {
            _loggerService.StartMethod();

            await new Migrator122(
                _applicationPropertyService,
                _preferencesService,
                _secureStorageService,
                _loggerService
                ).MigrateAsync();

            _loggerService.EndMethod();
        }

        public Task MigrateTo123Async()
        {
            _loggerService.StartMethod();

            // nothing to do

            _loggerService.EndMethod();

            return Task.CompletedTask;
        }

    }
}
