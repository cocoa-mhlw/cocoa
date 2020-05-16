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


        // GET /api/BeaconUuid - Beacon Uuid
        /*
        public async Task<BeaconUuidModel> GetBeaconUuidAsync()
        {
            try
            {
                string url = AppConstants.ApiBaseUrl + "/beaconUuid";
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var result = await Get(url);

                if (result != null)
                {
                    return Utils.DeserializeFromJson<BeaconUuidModel>(result);
                }
            }
            catch (HttpRequestException) { }

            return null;
        }
        */

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
                    UserDataModel userData = Utils.DeserializeFromJson<UserDataModel>(result);
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

        // GET /api/User/{userUuid}/{major}/{minor} - check user status and user exists
        public async Task<UserDataModel> GetUserAsync(UserDataModel user)
        {
            string url = AppConstants.ApiBaseUrl + $"/user/{user.UserUuid}/{user.Major}/{user.Minor}";
            var result = await Get(url);
            if (result != null)
            {
                return Utils.DeserializeFromJson<UserDataModel>(result);
            }
            return null;
        }

        // POST /api/Beacon - check user status and user exists
        /*
        public async Task<bool> PostBeaconDataAsync(UserDataModel user, BeaconDataModel beacon)
        {
            string url = AppConstants.ApiBaseUrl + "/Beacon";

            // TODO PostBeaconDataModel implementationa
            PostBeaconDataModel postBeaconDataModel = new PostBeaconDataModel();
            postBeaconDataModel.BeaconUuid = beacon.BeaconUuid;
            postBeaconDataModel.Count = beacon.Count;
            postBeaconDataModel.Distance = beacon.Distance;
            postBeaconDataModel.ElaspedTime = beacon.ElaspedTime;
            postBeaconDataModel.FirstDetectTime = beacon.FirstDetectTime;
            postBeaconDataModel.LastDetectTime = beacon.LastDetectTime;
            postBeaconDataModel.Major = beacon.Major;
            postBeaconDataModel.Minor = beacon.Minor;
            postBeaconDataModel.Rssi = beacon.Rssi;
            postBeaconDataModel.TXPower = beacon.TXPower;
            postBeaconDataModel.KeyTime = beacon.KeyTime;
            postBeaconDataModel.Id = beacon.Id;
            postBeaconDataModel.UserMajor = user.Major;
            postBeaconDataModel.UserMinor = user.Minor;
            postBeaconDataModel.UserUuid = user.UserUuid;

            HttpContent content = new StringContent(Utils.SerializeToJson(postBeaconDataModel), Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage result = await httpClient.PostAsync(url, content);
                await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
        */

        // POST /otp/send - send OTP to the specified phone number
        /*
        public Task PostOTPAsync(UserDataModel user, string phoneNumber)
        {
            string url = AppConstants.ApiBaseUrl + "/otp/send";
            HttpContent content = new StringContent(Utils.SerializeToJson(new { user, phone = phoneNumber }), Encoding.UTF8, "application/json");
            return Post(url, content);
        }
        */

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
    /*
    public class PostBeaconDataModel
    {
        /// <summary>
        /// auto number key
        /// </summary>
        /// <value>Id</value>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        [JsonProperty("userUuid")]
        public string UserUuid { get; set; }

        /// <summary>
        /// User Major 0 to 65536
        /// </summary>
        /// <value>User Major</value>
        [JsonProperty("userMajor")]
        public string UserMajor { get; set; }

        /// <summary>
        /// User Minor 0 to 65536
        /// </summary>
        /// <value>User Minor</value>
        [JsonProperty("userMinor")]
        public string UserMinor { get; set; }

        /// <summary>
        /// Same beacon uuid's device can communication count.
        /// </summary>
        /// <value>BeaconUuid</value>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// Same beacon uuid's device can communication.
        /// </summary>
        /// <value>BeaconUuid</value>
        [JsonProperty("beaconUuid")]
        public string BeaconUuid { get; set; }
        /// <summary>
        /// Major - in this app case mapping user id
        /// </summary>
        /// <value>BLE major number</value>
        [JsonProperty("major")]
        public string Major { get; set; }
        /// <summary>
        /// MInor - in this app case mapping user id
        /// </summary>
        /// <value>BLE minor number</value>
        [JsonProperty("minor")]
        public string Minor { get; set; }
        /// <summary>
        /// Bluetooth LE's calcated distance between beacons.
        /// </summary>
        /// <value>BLE minor number</value>
        [JsonProperty("distance")]
        public double Distance { get; set; }
        /// <summary>
        ///  Received Signal Strength Indicator, after use. Used for precise distance measurement
        /// </summary>
        /// <value>Received Signal Strength Indicator</value>
        [JsonProperty("rssi")]
        public int Rssi { get; set; }
        /// <summary>
        /// Power of beacon transmitter, after use. Used for precise distance measurement
        /// </summary>
        [JsonProperty("txPower")]
        public int TXPower { get; set; }
        /// <summary>
        /// Total contact time of the beacon, which makes a cumulative record.Consider the case where you go to the bathroom halfway and come back.
        /// </summary>
        [JsonProperty("elaspedTime")]
        public TimeSpan ElaspedTime { get; set; }
        /// <summary>
        /// The last time measured.
        /// </summary>
        [JsonProperty("lastDetectTime")]
        public DateTime LastDetectTime { get; set; }
        /// <summary>
        /// The first time measured.
        /// </summary>
        [JsonProperty("firstDetectTime")]
        public DateTime FirstDetectTime { get; set; }
        /// <summary>
        /// The splited timespan.
        /// </summary>
        [JsonProperty("keyTime")]
        public string KeyTime { get; set; }
    }
    */

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
    /*
    public class BeaconUuidModel
    {
        /// <summary>
        /// for Cosmos DB id
        /// </summary>
        public string id;
        /// <summary>
        /// Beacon Uuid
        /// </summary>
        [JsonProperty("beaconUuid")]
        public string BeaconUuid;
        /// <summary>
        /// created timestamp UTC
        /// </summary>
        [JsonProperty("createTime")]
        public DateTime CreateTime;
        /// <summary>
        /// Reloading Time UTC
        /// </summary>
        [JsonProperty("endTime")]
        public DateTime EndTime;
    }
    */
}
