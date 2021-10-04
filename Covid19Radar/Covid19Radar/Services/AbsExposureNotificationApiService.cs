/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public abstract class AbsExposureNotificationApiService : AbsExposureNotificationClient
    {
        private readonly ILoggerService _loggerService;

        public AbsExposureNotificationApiService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public async Task<bool> StartExposureNotificationAsync()
        {
            _loggerService.StartMethod();

            try
            {
                var enabled = await IsEnabledAsync();
                if (enabled)
                {
                    return false;
                }
                await StartAsync();
                return true;
            }
            finally
            {
                _loggerService.EndMethod();

            }
        }

        public async Task<bool> StopExposureNotificationAsync()
        {
            _loggerService.StartMethod();

            try
            {
                var enabled = await IsEnabledAsync();
                if (!enabled)
                {
                    return false;
                }
                await StopAsync();
                return true;

            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public class MockExposureNotificationApiService : AbsExposureNotificationApiService
    {
        private const int DUMMY_VERSION = 2;

        private bool _isEnabled = false;

        private readonly Random _random = new Random();

        public MockExposureNotificationApiService(
            ILoggerService loggerService
            )
            : base(loggerService)
        {
        }

        public override async Task<IList<ExposureNotificationStatus>> GetStatusesAsync()
        {
            IList<ExposureNotificationStatus> emptyList = new List<ExposureNotificationStatus>();
            return emptyList;
        }


        static TemporaryExposureKey GenerateRandomKey(int offsetDays, Random random)
        {
            var keyData = new byte[16];

            return new TemporaryExposureKey()
            {
                KeyData = keyData,
                RollingStartIntervalNumber = DateTimeOffset.UtcNow.AddDays(offsetDays).UtcDateTime.ToEnInterval(),
                RollingPeriod = random.Next(5, 144 + 1),
                RiskLevel = (RiskLevel)random.Next(1, 8 + 1),
                ReportType = (ReportType)random.Next(1, 8 + 1),
                DaysSinceOnsetOfSymptoms = random.Next(-14, 14 + 1)
            };
        }

        public override async Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
        {
            List<TemporaryExposureKey> temporaryExposureKeys = new List<TemporaryExposureKey>();

            for (var i = 1; i < 14; i++)
            {
                temporaryExposureKeys.Add(GenerateRandomKey(-i, _random));
            }

            return temporaryExposureKeys;
        }

        public override async Task<long> GetVersionAsync()
        {
            return DUMMY_VERSION;
        }

        public override async Task<bool> IsEnabledAsync()
        {
            return _isEnabled;
        }

        private (ExposureSummary, IEnumerable<ExposureInformation>) CreateDummyV1ExposureData()
        {
            var exposureSummary = new ExposureSummary()
            {
            };

            IEnumerable<ExposureInformation> CreateInformations()
            {
                var exposureInformations = new List<ExposureInformation>
                {
                    new ExposureInformation()
                    {
                        DateMillisSinceEpoch = DateTime.UtcNow.AddDays(-10).ToUnixEpochTime(),
                        DurationInMillis = TimeSpan.FromMinutes(5).Ticks,
                        AttenuationDurationsInMillis = new int[] { 1440000, 0, 0 },
                        AttenuationValue = 65,
                        TotalRiskScore = 5,
                        TransmissionRiskLevel = RiskLevel.Medium,
                    },
                    new ExposureInformation()
                    {
                        DateMillisSinceEpoch = DateTime.UtcNow.AddDays(-11).ToUnixEpochTime(),
                        DurationInMillis = TimeSpan.FromMinutes(5).Ticks,
                        AttenuationDurationsInMillis = new int[] { 1440000, 0, 0 },
                        AttenuationValue = 40,
                        TotalRiskScore = 3,
                        TransmissionRiskLevel = RiskLevel.Low,
                    },
                };
                return exposureInformations;
            }

            return (exposureSummary, CreateInformations());
        }

        public override async Task StartAsync()
        {
            _isEnabled = true;
        }

        public override async Task StopAsync()
        {
            _isEnabled = false;
        }

        public override Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            CancellationTokenSource cancellationTokenSource = null
            )
            => Task.FromResult(ProvideDiagnosisKeysResult.Completed);

        public override Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            ExposureConfiguration configuration,
            CancellationTokenSource cancellationTokenSource = null
            )
            => Task.FromResult(ProvideDiagnosisKeysResult.Completed);

        public override Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            ExposureConfiguration configuration,
            string token,
            CancellationTokenSource cancellationTokenSource = null
            )
            => Task.FromResult(ProvideDiagnosisKeysResult.Completed);

        public override Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync()
            => Task.CompletedTask;

        public override Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync()
            => Task.CompletedTask;

    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}
