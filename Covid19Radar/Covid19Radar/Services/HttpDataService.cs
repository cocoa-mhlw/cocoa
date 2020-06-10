using Covid19Radar.Common;
using Covid19Radar.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xamarin.ExposureNotifications;

namespace Covid19Radar.Services
{
    public class HttpDataService
    {
        private readonly HttpClient httpClient;
        private readonly HttpClient downloadClient;
        private string secret;
        public HttpDataService()
        {
            this.httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(AppConstants.ApiBaseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-functions-key", AppConstants.ApiSecret);
            SetSecret();
            this.downloadClient = new HttpClient();
        }

        private void SetSecret()
        {
            if (Application.Current.Properties.ContainsKey("Secret"))
            {
                secret = Application.Current.Properties["Secret"] as string;
            }
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AppConstants.ApiUserSecretKeyPrefix, secret);
        }

        //public bool HasSecret() => secret != null;


        // POST /api/Register - Register User
        public async Task<UserDataModel> PostRegisterUserAsync()
        {
            try
            {
                string url = AppConstants.ApiBaseUrl + "/register";
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
                    return userData;
                }
            }
            catch (HttpRequestException) { }

            return null;
        }

        // Put /diagnosis - upload self diagnosys file
        public async Task PutSelfExposureKeysAsync(SelfDiagnosisSubmission request)
        {
            System.Console.WriteLine(Utils.SerializeToJson(request));
            var url = $"{AppConstants.ApiBaseUrl.TrimEnd('/')}/diagnosis";
            var content = new StringContent(Utils.SerializeToJson(request), Encoding.UTF8, "application/json");
            var result = await PutAsync(url, content);
            if (result != null)
            {
                System.Console.WriteLine(Utils.SerializeToJson(result));
            }
        }

        public async Task<List<TemporaryExposureKeyExportFileModel>> GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken)
        {
            //long sinceEpochSeconds = new DateTimeOffset(DateTime.UtcNow.AddDays(-14)).ToUnixTimeSeconds();

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
            return await GetCdnStreamAsync(url,cancellationToken);
        }


        // GET /api/notification/pull/{lastClientUpdateTime:datetime} - pull Notifications 
        public async Task<NotificationPullResult> GetNotificationPullAsync(UserDataModel user)
        {
            string url = AppConstants.ApiBaseUrl
                + $"/notification/pull/{user.LastNotificationTime.ToString("yyyy-MM-ddTHH:mm:ss")}";
            var result = await GetAsync(url);
            if (result != null)
            {
                return Utils.DeserializeFromJson<NotificationPullResult>(result);
            }
            return null;
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

        private async Task<Stream> GetCdnStreamAsync(string url,CancellationToken cancellationToken)
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

        private async Task<string> PutAsync(string url, HttpContent body)
        {
            var result = await httpClient.PutAsync(url, body);
            await result.Content.ReadAsStringAsync();
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }

    }
}
