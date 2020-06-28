using Covid19Radar.Common;
using Covid19Radar.Model;
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
using Xamarin.Essentials;

namespace Covid19Radar.Services
{
    public class HttpDataService: IHttpDataService
    {
        private readonly HttpClient httpClient;
        private readonly HttpClient downloadClient;
        private string secret;
        public HttpDataService()
        {
            this.httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(AppSettings.Instance.ApiUrlBase);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);
            SetSecret();
            this.downloadClient = new HttpClient();
        }

        private void SetSecret()
        {
            var storedSecret = SecureStorage.GetAsync(AppConstants.StorageKey.Secret).Result;
            if (storedSecret != null)
            {
                secret = storedSecret;
            }
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secret);
        }

        //public bool HasSecret() => secret != null;


        // POST /api/Register - Register User
        public async Task<UserDataModel> PostRegisterUserAsync()
        {
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
                    await SecureStorage.SetAsync(AppConstants.StorageKey.Secret, registerResult.Secret);
                    SetSecret();
                    return userData;
                }
            }
            catch (HttpRequestException) { }

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
            string container = AppSettings.Instance.BlobStorageContainerName;
            string url = AppSettings.Instance.CdnUrlBase + $"{container}/{region}/list.json";
            var result = await GetCdnAsync(url, cancellationToken);
            if (result != null)
            {
                return Utils.DeserializeFromJson<List<TemporaryExposureKeyExportFileModel>>(result);
            }
            return new List<TemporaryExposureKeyExportFileModel>();
        }

        public async Task<Stream> GetTemporaryExposureKey(string url, CancellationToken cancellationToken)
        {
            return await GetCdnStreamAsync(url, cancellationToken);
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
            Task<HttpResponseMessage> response = downloadClient.GetAsync(url, cancellationToken);
            HttpResponseMessage result = await response;
            await result.Content.ReadAsStreamAsync();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStreamAsync();
            }
            return null;
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
