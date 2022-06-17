﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Covid19Radar.Repository;
using System.Linq;

namespace Covid19Radar.Services
{
    public class HttpDataService : IHttpDataService
    {
        private readonly ILoggerService loggerService;
        private readonly IServerConfigurationRepository serverConfigurationRepository;

        private readonly HttpClient apiClient;
        private readonly HttpClient httpClient;

        public HttpDataService(
            ILoggerService loggerService,
            IHttpClientService httpClientService,
            IServerConfigurationRepository serverConfigurationRepository
            )
        {
            this.loggerService = loggerService;
            this.serverConfigurationRepository = serverConfigurationRepository;

            apiClient = httpClientService.CreateApiClient();
            httpClient = httpClientService.CreateHttpClient();
        }

        // POST /api/Register - Register User
        public async Task<HttpStatusCode> PostRegisterUserAsync()
        {
            loggerService.StartMethod();
            try
            {
                await serverConfigurationRepository.LoadAsync();

                string url = serverConfigurationRepository.UserRegisterApiEndpoint;
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                HttpResponseMessage result = await httpClient.PostAsync(url, content);
                loggerService.EndMethod();
                return result.StatusCode;
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed to register user.", ex);
                loggerService.EndMethod();
                throw;
            }
        }

        public async Task<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
        {
            loggerService.StartMethod();

            try
            {
                await serverConfigurationRepository.LoadAsync();

                var diagnosisKeyRegisterApiUrls = serverConfigurationRepository.DiagnosisKeyRegisterApiUrls;
                if (diagnosisKeyRegisterApiUrls.Count() == 0)
                {
                    loggerService.Error("DiagnosisKeyRegisterApiUrls count 0");
                    throw new InvalidOperationException("DiagnosisKeyRegisterApiUrls count 0");
                }
                else if (diagnosisKeyRegisterApiUrls.Count() > 1)
                {
                    loggerService.Warning("Multi DiagnosisKeyRegisterApiUrl are detected.");
                }

                var url = diagnosisKeyRegisterApiUrls.First();
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                return await PutAsync(url, content);
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        public async Task<ApiResponse<LogStorageSas>> GetLogStorageSas()
        {
            loggerService.StartMethod();

            int statusCode;
            LogStorageSas logStorageSas = default;

            try
            {
                await serverConfigurationRepository.LoadAsync();

                var url = serverConfigurationRepository.InquiryLogApiUrl;

                var response = await apiClient.GetAsync(url);

                statusCode = (int)response.StatusCode;
                loggerService.Info($"Response status: {statusCode}");

                if (statusCode == (int)HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    logStorageSas = JsonConvert.DeserializeObject<LogStorageSas>(content);
                }
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed get log storage SAS.", ex);
                statusCode = 0;
                logStorageSas = default;
            }
            loggerService.EndMethod();
            return new ApiResponse<LogStorageSas>(statusCode, logStorageSas);
        }

        private async Task<HttpStatusCode> PutAsync(string url, HttpContent body)
        {
            var result = await httpClient.PutAsync(url, body);
            await result.Content.ReadAsStringAsync();
            return result.StatusCode;
        }
    }
}
