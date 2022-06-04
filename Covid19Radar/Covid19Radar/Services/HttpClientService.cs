/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Covid19Radar.Services
{
    public interface IHttpClientService
    {
        HttpClient Create();

        HttpClient CreateApiClient();

        HttpClient CreateHttpClient();

        HttpClient CreateCdnClient();
    }

    public class HttpClientService : IHttpClientService
    {
        public HttpClientService()
        {
        }

        public HttpClient CreateApiClient()
        {
            var apiClient = Create();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            apiClient.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);
            apiClient.DefaultRequestHeaders.Add("x-api-key", AppSettings.Instance.ApiKey);

            return apiClient;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = Create();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);

            return httpClient;
        }

        public HttpClient CreateCdnClient()
        {
            return new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
        }

        public HttpClient Create()
        {
            return new HttpClient();
        }
    }
}
