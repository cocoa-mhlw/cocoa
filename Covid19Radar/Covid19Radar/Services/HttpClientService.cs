/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Covid19Radar.Services
{
    public interface IHttpClientService
    {
        public HttpClient ApiClient
        {
            get;
        }

        public HttpClient HttpClient
        {
            get;
        }

        public HttpClient CdnClient
        {
            get;
        }
    }

    public class HttpClientService : IHttpClientService
    {
        private const double TimeoutSeconds = 10.0;

        private readonly HttpClient _apiClient;
        private readonly HttpClient _httpClient;
        private readonly HttpClient _cdnClient;

        public HttpClient ApiClient => _apiClient;
        public HttpClient HttpClient => _httpClient;
        public HttpClient CdnClient => _cdnClient;

        public HttpClientService()
        {
            _apiClient = CreateApiClient();
            _httpClient = CreateHttpClient();
            _cdnClient = CreateCdnClient();
        }

        private HttpClient CreateApiClient()
        {
            var apiClient = new HttpClient();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            apiClient.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);
            apiClient.DefaultRequestHeaders.Add("x-api-key", AppSettings.Instance.ApiKey);

            return apiClient;
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);
            httpClient.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);

            return httpClient;
        }

        private HttpClient CreateCdnClient()
        {
            var cdnClient = new HttpClient();
            return cdnClient;
        }
    }
}
