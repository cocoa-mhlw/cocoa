// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

namespace Covid19Radar.Services
{
    public interface ICheckVersionService
    {
        public Task CheckVersionAsync();
    }

    public class CheckVersionServiceNop : ICheckVersionService
    {
        public Task CheckVersionAsync()
            => Task.CompletedTask;
    }

    public class CheckVersionService : ICheckVersionService
    {
        private readonly HttpClient _httpClient;
        private readonly IEssentialsService _essentialsService;
        private readonly ILoggerService _loggerService;

        public CheckVersionService(
            IHttpClientService httpClientService,
            IEssentialsService essentialsService,
            ILoggerService loggerService
            )
        {
            _httpClient = httpClientService.Create();
            _essentialsService = essentialsService;
            _loggerService = loggerService;
        }

        public async Task CheckVersionAsync()
        {
            _loggerService.StartMethod();

            var uri = AppResources.UrlVersion;

            try
            {
                //var json = await _httpClient.GetStringAsync(uri);
                //var versionString = JObject.Parse(json).Value<string>("version");
                //if (new Version(versionString).CompareTo(new Version(_essentialsService.AppVersion)) > 0)
                //{
                if(true)
                {
                    await UserDialogs.Instance.AlertAsync(
                        AppResources.AppUtilsGetNewVersionDescription,
                        AppResources.AppUtilsGetNewVersionTitle,
                        AppResources.ButtonOk
                        );
                    await Browser.OpenAsync(_essentialsService.StoreUrl, BrowserLaunchMode.External);
                }

            }
            catch (Exception exception)
            {
                _loggerService.Exception("Failed to check version.", exception);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

    }
}
