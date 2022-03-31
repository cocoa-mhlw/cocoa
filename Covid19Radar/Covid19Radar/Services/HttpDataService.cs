/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Net.Http;
using System.Net.Http.Headers;

namespace Covid19Radar.Services
{
    class HttpDataService : IHttpDataService
    {
        private readonly IHttpClientService httpClientService;

        private readonly HttpClient _apiClient;
        private readonly HttpClient _httpClient;
        private readonly HttpClient _cdnClient;

        public HttpClient ApiClient => _apiClient;
        public HttpClient HttpClient => _httpClient;
        public HttpClient CdnClient => _cdnClient;

        public HttpDataService(
            IHttpClientService httpClientService
            )
        {
            this.httpClientService = httpClientService;

            _apiClient = CreateApiClient();
            _httpClient = CreateHttpClient();
            _cdnClient = CreateCdnClient();
        }

        private HttpClient CreateApiClient()
        {
            var apiClient = httpClientService.Create();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            apiClient.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);
            apiClient.DefaultRequestHeaders.Add("x-api-key", AppSettings.Instance.ApiKey);

            return apiClient;
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = httpClientService.Create();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);

            return httpClient;
        }

        private HttpClient CreateCdnClient()
        {
            var cdnClient = httpClientService.Create();
            return cdnClient;
        }

    }
}
