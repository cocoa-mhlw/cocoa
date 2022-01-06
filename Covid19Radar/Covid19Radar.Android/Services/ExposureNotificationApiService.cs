/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Common.Apis;
using Chino;
using Chino.Android.Google;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services
{
    public class ExposureNotificationApiService : AbsExposureNotificationApiService
    {
        public const int REQUEST_EN_START = 0x10;
        public const int REQUEST_GET_TEK_HISTORY = 0x11;
        public const int REQUEST_PREAUTHORIZE_KEYS = 0x12;

        public readonly ExposureNotificationClient Client = new ExposureNotificationClient();

        public ExposureNotificationApiService(
            ILoggerService loggerService
            ) : base(loggerService)
        {
        }

        public override async Task StartAsync()
        {
            await Client.StartAsync();
        }

        public override async Task<bool> StartExposureNotificationAsync()
        {
            try
            {
                await StartAsync();
                return true;
            }
            catch (ApiException apiException)
            {
                if (apiException.StatusCode == CommonStatusCodes.ResolutionRequired)
                {
                    apiException.Status.StartResolutionForResult(Platform.CurrentActivity, REQUEST_EN_START);
                    return false;
                }
                else
                {
                    throw apiException;
                }
            }
        }

        public override async Task StopAsync() => await Client.StopAsync();

        public override async Task<IList<ExposureNotificationStatus>> GetStatusesAsync()
        {
            return await Client.GetStatusesAsync();
        }

        public override async Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
        {
            try
            {
                return await Client.GetTemporaryExposureKeyHistoryAsync();
            }
            catch (ApiException apiException)
            {
                if (apiException.StatusCode == CommonStatusCodes.ResolutionRequired)
                {
                    apiException.Status.StartResolutionForResult(Platform.CurrentActivity, REQUEST_GET_TEK_HISTORY);
                    throw new ENException(ENException.Code_Android.FAILED_UNAUTHORIZED,
                        "GetTemporaryExposureKeyHistoryAsync StartResolutionForResult");
                }
                else
                {
                    throw apiException;
                }
            }
        }

        public override async Task<long> GetVersionAsync()
        {
            return await Client.GetVersionAsync();
        }

        public override async Task<bool> IsEnabledAsync()
        {
            return await Client.IsEnabledAsync();
        }

        public override async Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            CancellationTokenSource cancellationTokenSource = null
            )
            => await Client.ProvideDiagnosisKeysAsync(keyFiles, cancellationTokenSource);

        public override async Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            ExposureConfiguration configuration,
            CancellationTokenSource cancellationTokenSource = null
            )
            => await Client.ProvideDiagnosisKeysAsync(keyFiles, configuration, cancellationTokenSource);

        public override async Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            ExposureConfiguration configuration, string token,
            CancellationTokenSource cancellationTokenSource = null
            )
            => await Client.ProvideDiagnosisKeysAsync(keyFiles, configuration, token, cancellationTokenSource);

        public override async Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync()
        {
            try
            {
                await Client.RequestPreAuthorizedTemporaryExposureKeyHistoryAsync();
            }
            catch (ApiException apiException)
            {
                if (apiException.StatusCode == CommonStatusCodes.ResolutionRequired)
                {
                    apiException.Status.StartResolutionForResult(Platform.CurrentActivity, REQUEST_PREAUTHORIZE_KEYS);
                    throw new ENException(ENException.Code_Android.FAILED_UNAUTHORIZED,
                        "RequestPreAuthorizedTemporaryExposureKeyHistoryAsync StartResolutionForResult");
                }
                else
                {
                    throw apiException;
                }
            }
        }

        public override async Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync()
        {
            try
            {
                await Client.RequestPreAuthorizedTemporaryExposureKeyReleaseAsync();
            }
            catch (ApiException apiException)
            {
                if (apiException.StatusCode == CommonStatusCodes.ResolutionRequired)
                {
                    apiException.Status.StartResolutionForResult(Platform.CurrentActivity, REQUEST_PREAUTHORIZE_KEYS);
                    throw new ENException(ENException.Code_Android.FAILED_UNAUTHORIZED,
                        "RequestPreAuthorizedTemporaryExposureKeyReleaseAsync StartResolutionForResult");
                }
                else
                {
                    throw apiException;
                }
            }
        }
    }
}
