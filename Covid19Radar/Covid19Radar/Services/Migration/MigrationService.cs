/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Services.Logs;

#nullable enable
namespace Covid19Radar.Services.Migration
{

    /// <summary>
    /// Define the version that need migration.
    /// </summary>
    public interface IMigrationProcessService
    {
        /// <summary>
        /// Setup migration.
        ///
        /// This method will be executed every migration process once.
        /// </summary>
        /// <returns></returns>
        public Task SetupAsync() => Task.CompletedTask;

        public Task Initialize_1_0_0_Async() => Task.CompletedTask;

        public Task MigrateTo_1_2_2_Async() => Task.CompletedTask;

        public Task MigrateTo_1_2_3_Async() => Task.CompletedTask;
    }

    public interface IMigrationService
    {
        public Task MigrateAsync();
    }

    public class MigrationService : IMigrationService
    {
        private const string FIRST_VERSION = "1.0.0";

        private static readonly Version VERSION_1_0_0 = new Version(FIRST_VERSION);
        private static readonly Version VERSION_1_2_2 = new Version("1.2.2");
        private static readonly Version VERSION_1_2_3 = new Version("1.2.3");

        private static readonly Version VERSION_LATEST = VERSION_1_2_3;

        private void SetPreferenceVersion(Version version)
            => _preferencesService.SetValue(PreferenceKey.AppVersion, version.ToString());

        private Version? GetPreferenceVersion()
        {
            _loggerService.StartMethod();

            if (!_preferencesService.ContainsKey(PreferenceKey.AppVersion))
            {
                _loggerService.Debug($"appVersion entry is not found in Preferences.");
                return null;
            }
            var appVersion = _preferencesService.GetValue(PreferenceKey.AppVersion, FIRST_VERSION);
            _loggerService.Info($"Current Preference Version: {appVersion}");

            _loggerService.EndMethod();

            return new Version(appVersion);
        }

        private readonly IMigrationProcessService _platformMigrationProcessService;
        private readonly IApplicationPropertyService _applicationPropertyService;
        private readonly IPreferencesService _preferencesService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly IEssentialsService _essentialsService;
        private readonly ILoggerService _loggerService;

        private readonly SemaphoreSlim _semaphoreForMigrate = new SemaphoreSlim(1, 1);

        public MigrationService(
            IMigrationProcessService platformMigrationProcessService,
            IApplicationPropertyService applicationPropertyService,
            IPreferencesService preferencesService,
            ISecureStorageService secureStorageService,
            IEssentialsService essentialsService,
            ILoggerService loggerService
            )
        {
            _platformMigrationProcessService = platformMigrationProcessService;
            _applicationPropertyService = applicationPropertyService;
            _preferencesService = preferencesService;
            _secureStorageService = secureStorageService;
            _essentialsService = essentialsService;
            _loggerService = loggerService;
        }

        private Version? _currentAppVersion;
        private Version GetCurrentAppVersion()
        {
            if (_currentAppVersion != null)
            {
                return _currentAppVersion;
            }

            string appVersion = _essentialsService.AppVersion;

            try
            {
                _currentAppVersion = new Version(appVersion);
            }
            catch (ArgumentException e)
            {
                _loggerService.Exception($"AppVersion {appVersion} is not valid.", e);
                _currentAppVersion = VERSION_LATEST;
            }

            return _currentAppVersion;
        }

        public async Task MigrateAsync()
        {
            _loggerService.StartMethod();

            await _semaphoreForMigrate.WaitAsync();

            _loggerService.Info("Migration process have been started.");

            try
            {
                var fromVersion = GetPreferenceVersion();
                await MigrateAsync(fromVersion);
            }
            finally
            {
                _loggerService.Info("Migration process completed.");

                _semaphoreForMigrate.Release();

                _loggerService.EndMethod();
            }
        }

        private bool DetectDowngrade()
        {
            var fromVersion = GetPreferenceVersion() ?? GuessVersion();
            return fromVersion.CompareTo(GetCurrentAppVersion()) > 0;
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

            if (DetectDowngrade())
            {
                _loggerService.Error("App version Downgrade detected.");
            }

            if (fromVersion is null)
            {
                fromVersion = GuessVersion();
                if (fromVersion.CompareTo(VERSION_1_0_0) == 0)
                {
                    await new Initializer_1_0_0(
                        _applicationPropertyService,
                        _loggerService
                        ).ExecuteAsync();
                    SetPreferenceVersion(VERSION_1_0_0);
                }
            }

            if (fromVersion.CompareTo(GetCurrentAppVersion()) == 0)
            {
                _loggerService.Debug($"fromVersion: {fromVersion} == currentVersion: {GetCurrentAppVersion()}");
                _loggerService.EndMethod();
                return;
            }

            await _platformMigrationProcessService.SetupAsync();

            if (fromVersion.CompareTo(VERSION_1_2_2) < 0)
            {
                await new Migrator_1_2_2(
                    _applicationPropertyService,
                    _preferencesService,
                    _secureStorageService,
                    _loggerService
                    ).ExecuteAsync();

                await _platformMigrationProcessService.MigrateTo_1_2_2_Async();

                SetPreferenceVersion(VERSION_1_2_2);
            }

            if (fromVersion.CompareTo(VERSION_1_2_3) < 0)
            {
                await _platformMigrationProcessService.MigrateTo_1_2_3_Async();

                SetPreferenceVersion(VERSION_1_2_3);
            }

            SetPreferenceVersion(GetCurrentAppVersion());

            _loggerService.EndMethod();
        }
    }
}
#nullable disable
