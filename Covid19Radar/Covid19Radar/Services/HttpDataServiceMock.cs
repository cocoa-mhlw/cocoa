/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net.Http;
using Covid19Radar.Common;

namespace Covid19Radar.Services
{
    class HttpDataServiceMock : IHttpDataService
    {
        private readonly HttpClient downloadClient;
        public HttpDataServiceMock(IHttpClientService httpClientService)
        {
            if (DownloadRequired())
            {
                downloadClient = httpClientService.Create();
            }
        }

        // copy from ./HttpDataService.cs
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

        public Task MigrateFromUserData(UserDataModel userData)
        {
            return Task.CompletedTask;
        }

        public (string, string) GetCredentials()
        {
            return ("user-uuid", "secret");
        }

        public void RemoveCredentials()
        {
        }

        Task<Stream> IHttpDataService.GetTemporaryExposureKey(string url, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew<Stream>(() =>
            {
                Debug.WriteLine("HttpDataServiceMock::GetTemporaryExposureKey called");
                return new MemoryStream();
            });
        }

        private TemporaryExposureKeyExportFileModel TestDat(long created)
        {
            var tmp = new TemporaryExposureKeyExportFileModel();
            tmp.Region = "440";
            tmp.Url = "testUrl";
            tmp.Created = created;
            return (tmp);
        }

        private long TimeBefore(int day)
        {
            return (new DateTimeOffset(DateTime.UtcNow.ToLocalTime().AddDays(day)).ToUnixTimeMilliseconds());
        }

        private long NightBefore(int day)
        {
            DateTime d = DateTime.UtcNow.ToLocalTime().AddDays(day);
            return (new DateTimeOffset(new DateTime(d.Year, d.Month, d.Day, 0, 1, 2, 3)).ToUnixTimeMilliseconds());
        }

        private List<TemporaryExposureKeyExportFileModel> DataPreset(int dataVer)
        {
            /* DataPreset for TEK FileModel
               0(default): nothing (default for v1.2.3)
               1: real time
               2: last night
               please add
             */
            switch (dataVer)
            {
                case 2:
                    return new List<TemporaryExposureKeyExportFileModel> { TestDat(NightBefore(-1)), TestDat(NightBefore(0)) };
                case 1:
                    return new List<TemporaryExposureKeyExportFileModel> { TestDat(TimeBefore(-1)), TestDat(TimeBefore(0)) };
                case 0:
                default:
                    return new List<TemporaryExposureKeyExportFileModel>();
            }
        }

        private List<TemporaryExposureKeyExportFileModel> Data()
        {
            int dataVer = -1;
            string url = AppSettings.Instance.CdnUrlBase;
            if (Regex.IsMatch(url, @"^(\d+,)+\d+,*$"))
            {
                return (url.Split(",").ToList().Select(x => TestDat(Convert.ToInt64(x))).ToList());
            }
            Match match = Regex.Match(url, @"https://CDN_URL_BASE/(?<d>\d+?)");
            if (match.Success)
            {
                dataVer = Convert.ToUInt16(match.Groups["d"].Value);
            }
            return (DataPreset(dataVer));
        }

        private static bool DownloadRequired()
        {
            string url = AppSettings.Instance.CdnUrlBase;
            return (Regex.IsMatch(url, @"^https://.*\..*\..*/$"));
        }

        async Task<List<TemporaryExposureKeyExportFileModel>> IHttpDataService.GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken)
        {
            /* CdnUrlBase trick for Debug_Mock
               "https://www.example.com/"(url with 2+ periods) -> download "url"+"c19r/440/list.json"
               "1598022036649,1598022036751,1598022036826" -> direct input timestamps 
               "https://CDN_URL_BASE/2" -> dataVer = 2
               "https://CDN_URL_BASE/" -> dataVer = 0 (default)
            */
            if (DownloadRequired())
            {
                // copy from GetTemporaryExposureKeyList @ ./HttpDataService.cs and delete logger part
                string container = AppSettings.Instance.BlobStorageContainerName;
                string url = AppSettings.Instance.CdnUrlBase + $"{container}/{region}/list.json";
                var result = await GetCdnAsync(url, cancellationToken);
                if (result != null)
                {
                    Debug.WriteLine("HttpDataServiceMock::GetTemporaryExposureKeyList downloaded");
                    return Utils.DeserializeFromJson<List<TemporaryExposureKeyExportFileModel>>(result);
                }
                else
                {
                    Debug.WriteLine("HttpDataServiceMock::GetTemporaryExposureKeyList download failed");
                    return new List<TemporaryExposureKeyExportFileModel>();
                }
            }

            Debug.WriteLine("HttpDataServiceMock::GetTemporaryExposureKeyList called");
            return Data();
        }

        async Task<bool> IHttpDataService.PostRegisterUserAsync()
        {
            Debug.WriteLine("HttpDataServiceMock::PostRegisterUserAsync called");
            return await Task.FromResult(true);
        }

        Task<HttpStatusCode> IHttpDataService.PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
        {
            return Task.Factory.StartNew<HttpStatusCode>(() =>
            {
                Debug.WriteLine("HttpDataServiceMock::PutSelfExposureKeysAsync called");
                return HttpStatusCode.OK;
            });
        }

        public Task<ApiResponse<LogStorageSas>> GetLogStorageSas()
        {
            return Task.Factory.StartNew(() =>
            {
                Debug.WriteLine("HttpDataServiceMock::GetStorageKey called");
                return new ApiResponse<LogStorageSas>((int)HttpStatusCode.OK, new LogStorageSas { SasToken = "sv=2012-02-12&se=2015-07-08T00%3A12%3A08Z&sr=c&sp=wl&sig=t%2BbzU9%2B7ry4okULN9S0wst%2F8MCUhTjrHyV9rDNLSe8g%3Dsss" });
            });
        }
    }
}
