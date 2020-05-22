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

namespace Covid19Radar.Services
{
    public class HttpDataService
    {
        private readonly HttpClient httpClient;
        private string secret;
        public HttpDataService()
        {
            this.httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(AppConstants.ApiBaseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-functions-key", AppConstants.ApiSecret);
            SetSecret();
        }

        private void SetSecret()
        {
            if (Application.Current.Properties.ContainsKey("Secret"))
            {
                secret = Application.Current.Properties["Secret"] as string;
            }
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AppConstants.ApiUserSecretKeyPrefix, secret);
        }

        public bool HasSecret() => secret != null;


        // POST /api/Register - Register User
        public async Task<UserDataModel> PostRegisterUserAsync()
        {
            try
            {
                string url = AppConstants.ApiBaseUrl + "/register";
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var result = await Post(url, content);
                if (result != null)
                {
                    var userData = Utils.DeserializeFromJson<UserDataModel>(result);
                    secret = userData.Secret;
                    Application.Current.Properties["Secret"] = secret;
                    await Application.Current.SavePropertiesAsync();
                    SetSecret();
                    return userData;
                }
            }
            catch (HttpRequestException) { }

            return null;
        }

        // GET /api/User/{userUuid} - check user status and user exists
        public async Task<UserDataModel> GetUserAsync(UserDataModel user)
        {
            string url = AppConstants.ApiBaseUrl + $"/user/{user.UserUuid}";
            var result = await Get(url);
            if (result != null)
            {
                return Utils.DeserializeFromJson<UserDataModel>(result);
            }
            return null;
        }


        // GET /api/notification/pull/{lastClientUpdateTime:datetime} - pull Notifications 
        public async Task<NotificationPullResult> GetNotificationPullAsync(UserDataModel user)
        {
            string url = AppConstants.ApiBaseUrl 
                + $"/notification/pull/{user.LastNotificationTime.ToString("yyyy-MM-ddTHH:mm:ss")}";
            var result = await Get(url);
            if (result != null)
            {
                return Utils.DeserializeFromJson<NotificationPullResult>(result);
            }
            return null;
        }


        private async Task<string> Get(string url)
        {
            Task<HttpResponseMessage> stringAsync = httpClient.GetAsync(url);
            HttpResponseMessage result = await stringAsync;
            await result.Content.ReadAsStringAsync();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }

        private async Task<string> Post(string url, HttpContent body)
        {
            HttpResponseMessage result = await httpClient.PostAsync(url, body);
            await result.Content.ReadAsStringAsync();
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }

        private async Task<string> Put(string url, HttpContent body)
        {
            var result = await httpClient.PutAsync(url, body);
            await result.Content.ReadAsStringAsync();
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }

    }

    public class NotificationPullResult
    {
        /// <summary>
        /// Last notification date and time
        /// </summary>
        [JsonProperty("lastNotificationTime")]
        public DateTime LastNotificationTime { get; set; }
        /// <summary>
        /// Notification Messages
        /// </summary>
        [JsonProperty("messages")]
        public NotificationMessageModel[] Messages { get; set; }
    }
}
