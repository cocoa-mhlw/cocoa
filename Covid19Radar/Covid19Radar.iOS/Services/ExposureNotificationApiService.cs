/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chino;
using Chino.iOS;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.iOS.Services
{
    public class ExposureNotificationApiService : AbsExposureNotificationApiService
    {
        private ExposureNotificationClient _exposureNotificationClient = new ExposureNotificationClient();

        public string UserExplanation
        {
            set => _exposureNotificationClient.UserExplanation = value;
        }

        public ExposureNotificationApiService(
            ILoggerService loggerService
            ) : base(loggerService)
        {
        }

        public override Task<IList<ExposureNotificationStatus>> GetStatusesAsync()
            => _exposureNotificationClient.GetStatusesAsync();

        public override Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
            => _exposureNotificationClient.GetTemporaryExposureKeyHistoryAsync();

        public override Task<long> GetVersionAsync()
            => _exposureNotificationClient.GetVersionAsync();

        public override async Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            CancellationTokenSource cancellationTokenSource = null
            )
            => await _exposureNotificationClient.ProvideDiagnosisKeysAsync(keyFiles, cancellationTokenSource);

        public override async Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            string token,
            CancellationTokenSource cancellationTokenSource = null
            )
            => await _exposureNotificationClient.ProvideDiagnosisKeysAsync(keyFiles, token, cancellationTokenSource);

        public override Task<bool> IsEnabledAsync()
            => _exposureNotificationClient.IsEnabledAsync();

        public override Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync()
            => _exposureNotificationClient.RequestPreAuthorizedTemporaryExposureKeyHistoryAsync();

        public override Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync()
            => _exposureNotificationClient.RequestPreAuthorizedTemporaryExposureKeyReleaseAsync();

        public override Task StartAsync()
            => _exposureNotificationClient.StartAsync();

        public override Task StopAsync()
            => _exposureNotificationClient.StopAsync();
    }
}
