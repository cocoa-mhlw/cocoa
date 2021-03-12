using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Net;
using Newtonsoft.Json;

namespace Covid19Radar.Services
{
    public class HttpDataService : IHttpDataService
    {
        private readonly ILoggerService loggerService;
        private readonly ISecureStorageService secureStorageService;
        private readonly IApplicationPropertyService applicationPropertyService;

        private readonly HttpClient apiClient; // API key based client.
        private readonly HttpClient httpClient;
        private readonly HttpClient downloadClient;

        public HttpDataService(ILoggerService loggerService, IHttpClientService httpClientService, ISecureStorageService secureStorageService, IApplicationPropertyService applicationPropertyService)
        {
            this.loggerService = loggerService;
            this.secureStorageService = secureStorageService;
            this.applicationPropertyService = applicationPropertyService;

            // Create API key based client.
            apiClient = httpClientService.Create();
            apiClient.BaseAddress = new Uri(AppSettings.Instance.ApiUrlBase);
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            apiClient.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);
            apiClient.DefaultRequestHeaders.Add("x-api-key", AppSettings.Instance.ApiKey);

            // Create client.
            httpClient = httpClientService.Create();
            httpClient.BaseAddress = new Uri(AppSettings.Instance.ApiUrlBase);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);

            // Create download client.
            downloadClient = httpClientService.Create();
        }

        // POST /api/Register - Register User
        public async Task<bool> PostRegisterUserAsync()
        {
#if TEST_BACKTASK
            loggerService.StartMethod();
            loggerService.Debug(" skip /register ");
            loggerService.EndMethod();
            return true;
#endif

            loggerService.StartMethod();
            try
            {
                string url = AppSettings.Instance.ApiUrlBase + "/register";
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var result = await PostAsync(url, content);
                if (result != null)
                {
                    loggerService.EndMethod();
                    return true;
                }
            }
            catch (HttpRequestException ex)
            {
                loggerService.Exception("Failed to register user.", ex);
            }

            loggerService.EndMethod();
            return false;
        }

        public async Task<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
        {
            loggerService.StartMethod();
            var url = $"{AppSettings.Instance.ApiUrlBase.TrimEnd('/')}/v2/diagnosis";
            var content = new StringContent(Utils.SerializeToJson(request), Encoding.UTF8, "application/json");
            HttpStatusCode status = await PutAsync(url, content);
            loggerService.EndMethod();
            return status;
        }

        public async Task<List<TemporaryExposureKeyExportFileModel>> GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken)
        {
            loggerService.StartMethod();

            string container = AppSettings.Instance.BlobStorageContainerName;
            string url = AppSettings.Instance.CdnUrlBase + $"{container}/{region}/list.json";
            var result = await GetCdnAsync(url, cancellationToken);
            if (result != null)
            {
                loggerService.Info("Success to download");
                loggerService.EndMethod();
                return Utils.DeserializeFromJson<List<TemporaryExposureKeyExportFileModel>>(result);
            }
            else
            {
                loggerService.Error("Fail to download");
                loggerService.EndMethod();
                return new List<TemporaryExposureKeyExportFileModel>();
            }
        }

        public async Task<Stream> GetTemporaryExposureKey(string url, CancellationToken cancellationToken)
        {
            return await GetCdnStreamAsync(url, cancellationToken);
        }

        public async Task<ApiResponse<LogStorageSas>> GetLogStorageSas()
        {
            loggerService.StartMethod();

            int statusCode;
            LogStorageSas logStorageSas = default;

            try
            {
                var requestUrl = $"{AppSettings.Instance.ApiUrlBase.TrimEnd('/')}/inquirylog";
                var response = await apiClient.GetAsync(requestUrl);

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

        private async Task<string> GetAsync(string url)
        {
            Task<HttpResponseMessage> response = httpClient.GetAsync(url);
            HttpResponseMessage result = await response;
            await result.Content.ReadAsStringAsync();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }

        private async Task<string> GetAsync(string url, CancellationToken cancellationToken)
        {
            Task<HttpResponseMessage> response = httpClient.GetAsync(url, cancellationToken);
            HttpResponseMessage result = await response;
            await result.Content.ReadAsStringAsync();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }
        private async Task<string> GetCdnAsync(string url)
        {
            Task<HttpResponseMessage> response = downloadClient.GetAsync(url);
            HttpResponseMessage result = await response;
            await result.Content.ReadAsStringAsync();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }
        private async Task<string> GetCdnAsync(string url, CancellationToken cancellationToken)
        {
            Task<HttpResponseMessage> response = downloadClient.GetAsync(url, cancellationToken);
            HttpResponseMessage result = await response;
            await result.Content.ReadAsStringAsync();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }

        private async Task<Stream> GetCdnStreamAsync(string url)
        {
            Task<HttpResponseMessage> response = downloadClient.GetAsync(url);
            HttpResponseMessage result = await response;
            await result.Content.ReadAsStreamAsync();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStreamAsync();
            }
            return null;
        }

        private async Task<Stream> GetCdnStreamAsync(string url, CancellationToken cancellationToken)
        {
            loggerService.StartMethod();

            Task<HttpResponseMessage> response = downloadClient.GetAsync(url, cancellationToken);
            HttpResponseMessage result = await response;
            await result.Content.ReadAsStreamAsync();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                loggerService.Info("Success to download");
                loggerService.EndMethod();
                return await result.Content.ReadAsStreamAsync();
            }
            else
            {
                loggerService.Error("Fail to download");
                loggerService.EndMethod();
                return null;
            }
        }


        private async Task<string> PostAsync(string url, HttpContent body)
        {
            HttpResponseMessage result = await httpClient.PostAsync(url, body);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }

        private async Task<HttpStatusCode> PutAsync(string url, HttpContent body)
        {
            var result = await httpClient.PutAsync(url, body);
            await result.Content.ReadAsStringAsync();
            return result.StatusCode;
        }

    }
}
