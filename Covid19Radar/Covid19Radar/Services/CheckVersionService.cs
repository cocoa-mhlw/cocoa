// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json.Linq;

namespace Covid19Radar.Services
{
    public interface ICheckVersionService
    {
        public Task<bool> IsUpdateVersionExistAsync();
    }

    public class CheckVersionServiceNop : ICheckVersionService
    {
        public Task<bool> IsUpdateVersionExistAsync()
            => Task.FromResult(false);
    }

    public class CheckVersionService : ICheckVersionService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IEssentialsService _essentialsService;
        private readonly ILoggerService _loggerService;

        public CheckVersionService(
            IHttpClientService httpClientService,
            IEssentialsService essentialsService,
            ILoggerService loggerService
            )
        {
            _httpClientService = httpClientService;
            _essentialsService = essentialsService;
            _loggerService = loggerService;
        }

        public async Task<bool> IsUpdateVersionExistAsync()
        {
            _loggerService.StartMethod();

            var uri = AppResources.UrlVersion;
            try
            {
                var json = await _httpClientService.HttpClient.GetStringAsync(uri);
                var key = _essentialsService.IsIos ? "ios" : "android";
                var versionString = JObject.Parse(json).Value<string>(key);

                return new Version(versionString).CompareTo(new Version(_essentialsService.AppVersion)) > 0;
            }
            catch (Exception ex)
            {
                _loggerService.Exception("Failed to check version.", ex);
            }
            finally
            {
                _loggerService.EndMethod();
            }

            return false;
        }

    }
}
