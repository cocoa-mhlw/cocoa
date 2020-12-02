using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
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
        private readonly HttpClient apiClient; // API key based client.
        private readonly HttpClient httpClient; // Secret based client.
        private readonly HttpClient downloadClient;
        private string secret;
        public HttpDataService(ILoggerService loggerService)
        {
            this.loggerService = loggerService;

            // Create API key based client.
            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri(AppSettings.Instance.ApiUrlBase);
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            apiClient.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);
            apiClient.DefaultRequestHeaders.Add("x-api-key", AppSettings.Instance.ApiKey);

            // Create Secret based client.
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(AppSettings.Instance.ApiUrlBase);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);
            SetSecret();

            // Create download client.
            downloadClient = new HttpClient();
        }

        private void SetSecret()
        {
            if (Application.Current.Properties.ContainsKey("Secret"))
            {
                secret = Application.Current.Properties["Secret"] as string;
            }
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secret);
        }

        //public bool HasSecret() => secret != null;


        // POST /api/Register - Register User
        public async Task<UserDataModel> PostRegisterUserAsync()
        {
            loggerService.StartMethod();
            try
            {
                string url = AppSettings.Instance.ApiUrlBase + "/register";
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var result = await PostAsync(url, content);
                if (result != null)
                {
                    var registerResult = Utils.DeserializeFromJson<RegisterResultModel>(result);

                    UserDataModel userData = new UserDataModel();
                    userData.Secret = registerResult.Secret;
                    userData.UserUuid = registerResult.UserUuid;
                    userData.JumpConsistentSeed = registerResult.JumpConsistentSeed;
                    userData.IsOptined = true;
                    Application.Current.Properties["Secret"] = registerResult.Secret;
                    await Application.Current.SavePropertiesAsync();
                    SetSecret();

                    loggerService.EndMethod();
                    return userData;
                }
            }
            catch (HttpRequestException ex) {
                loggerService.Exception("Failed to register user.", ex);
            }

            loggerService.EndMethod();
            return null;
        }

        public async Task<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
        {
            var url = $"{AppSettings.Instance.ApiUrlBase.TrimEnd('/')}/diagnosis";
            var content = new StringContent(Utils.SerializeToJson(request), Encoding.UTF8, "application/json");
            HttpStatusCode status = await PutAsync(url, content);
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
            await result.Content.ReadAsStringAsync();
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
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
