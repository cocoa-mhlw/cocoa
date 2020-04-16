using Covid19Radar.Common;
using Covid19Radar.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{


    public class HttpDataService
    {
        private readonly HttpClient httpClient;

        public HttpDataService()
        {
            this.httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(AppConstants.ApiBaseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // POST /api/Register -  Register User 
        public async Task<UserDataModel> PostRegisterUserAsync()
        {
            string url = AppConstants.ApiBaseUrl + "/Register";
            var result = await Post(url, null);
            if (result != null)
            {
                return Utils.DeserializeFromJson<UserDataModel>(result);
            }
            return null;
        }

        // POST /api/User - check user status and user exists
        public async Task<UserDataModel> PostUserAsync(UserDataModel user)
        {
            string url = AppConstants.ApiBaseUrl + "/User";
            HttpContent content = new StringContent(Utils.SerializeToJson(user), Encoding.UTF8, "application/json");
            var result = await Post(url, content);
            if (result != null)
            {
                return Utils.DeserializeFromJson<UserDataModel>(result);
            }
            return null;
        }

        // PUT /api/User - update user status
        public async Task<UserDataModel> PutUserAsync(UserDataModel user)
        {
            string url = AppConstants.ApiBaseUrl + "/User";
            HttpContent content = new StringContent(Utils.SerializeToJson(user), Encoding.UTF8, "application/json");
            var result = await Put(url, content);
            if (result != null)
            {
                return Utils.DeserializeFromJson<UserDataModel>(result);
            }
            return null;
        }

        // POST /api/Beacon - check user status and user exists
        public async Task<UserDataModel> PostBeaconDataAsync(UserDataModel user, BeaconDataModel beacon)
        {
            string url = AppConstants.ApiBaseUrl + "/Beacon";

            // TODO PostBeaconDataModel implementationa
            PostBeaconDataModel postBeaconDataModel = new PostBeaconDataModel();
            postBeaconDataModel.BeaconUuid = beacon.BeaconUuid;
            postBeaconDataModel.Count = beacon.Count;
            postBeaconDataModel.Distance = beacon.Distance;
            postBeaconDataModel.ElaspedTime = beacon.ElaspedTime;
            postBeaconDataModel.LastDetectTime = beacon.LastDetectTime;
            postBeaconDataModel.Major = beacon.Major;
            postBeaconDataModel.Minor = beacon.Minor;
            postBeaconDataModel.Rssi = beacon.Rssi;
            postBeaconDataModel.TXPower = beacon.TXPower;
            postBeaconDataModel.UserMajor = user.Major;
            postBeaconDataModel.UserMinor = user.Minor;
            postBeaconDataModel.UserUuid = user.UserUuid;

            HttpContent content = new StringContent(Utils.SerializeToJson(postBeaconDataModel), Encoding.UTF8, "application/json");
            var result = await Post(url, content);
            if (result != null)
            {
                return Utils.DeserializeFromJson<UserDataModel>(result);
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
            Task<HttpResponseMessage> stringAsync = httpClient.PostAsync(url, body);
            HttpResponseMessage result = await stringAsync;
            await result.Content.ReadAsStringAsync();
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }
        private async Task<string> Put(string url, HttpContent body)
        {
            Task<HttpResponseMessage> stringAsync = httpClient.PutAsync(url, body);
            HttpResponseMessage result = await stringAsync;
            await result.Content.ReadAsStringAsync();
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }

    }
    public class PostBeaconDataModel
    {
        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        public string UserUuid { get; set; }

        /// <summary>
        /// User Major 0 to 65536
        /// </summary>
        /// <value>User Major</value>
        public string UserMajor { get; set; }

        /// <summary>
        /// User Minor 0 to 65536
        /// </summary>
        /// <value>User Minor</value>
        public string UserMinor { get; set; }

        /// <summary>
        /// Same beacon uuid's device can communication count.
        /// </summary>
        /// <value>BeaconUuid</value>
        public int Count { get; set; }

        /// <summary>
        /// Same beacon uuid's device can communication.
        /// </summary>
        /// <value>BeaconUuid</value>
        public string BeaconUuid { get; set; }
        /// <summary>
        /// Major - in this app case mapping user id
        /// </summary>
        /// <value>BLE major number</value>
        public string Major { get; set; }
        /// <summary>
        /// MInor - in this app case mapping user id
        /// </summary>
        /// <value>BLE minor number</value>
        public string Minor { get; set; }
        /// <summary>
        /// Bluetooth LE's calcated distance between beacons.
        /// </summary>
        /// <value>BLE minor number</value>
        public double Distance { get; set; }
        /// <summary>
        ///  Received Signal Strength Indicator, after use. Used for precise distance measurement
        /// </summary>
        /// <value>Received Signal Strength Indicator</value>
        public int Rssi { get; set; }
        /// <summary>
        /// Power of beacon transmitter, after use. Used for precise distance measurement
        /// </summary>
        public int TXPower { get; set; }
        /// <summary>
        /// Total contact time of the beacon, which makes a cumulative record.Consider the case where you go to the bathroom halfway and come back.
        /// </summary>
        public TimeSpan ElaspedTime { get; set; }
        /// <summary>
        /// The last time measured.
        /// </summary>
        public DateTime LastDetectTime { get; set; }

    }
}
