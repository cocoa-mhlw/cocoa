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
using System.Linq;
using System.Net.Http;
using Covid19Radar.Common;

namespace Covid19Radar.Services
{
    class HttpDataServiceMock : IHttpDataService
    {
        private readonly HttpClient downloadClient;
        private readonly MockCommonUtils mockCommonUtils;

        public HttpDataServiceMock(IHttpClientService httpClientService)
        {
            downloadClient = httpClientService.Create();
        }

        // copy from ./HttpDataService.cs
        private async Task<string> GetCdnAsync(string url, CancellationToken cancellationToken)
        {
            HttpResponseMessage result = await downloadClient.GetAsync(url, cancellationToken);
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

        private TemporaryExposureKeyExportFileModel CreateTestData(long created)
        {
            return new TemporaryExposureKeyExportFileModel()
            {
                Region = "440",
                Url = "testUrl",
                Created = created
            };
        }

        private long CalcTimeAddDays(int day)
          => new DateTimeOffset(DateTime.UtcNow.AddDays(day)).ToUnixTimeMilliseconds();

        private long CalcMidnightTimeAddDays(int day)
        {
            DateTime d = DateTime.UtcNow.AddDays(day);
            return (new DateTimeOffset(new DateTime(d.Year, d.Month, d.Day, 0, 1, 2, 3)).ToUnixTimeMilliseconds());
            // set 0 hour,1 min,2 sec,3 millisecond for debug
        }

        enum PresetTekListType // PresetTekListData for Tek List 
        {
            Nothing = 0, //nothing (default for v1.2.3)
            RealTime = 1, // real time
            MidnightTime = 2, // last night
            // please add "YourDataType = <int>"
        }

        private List<TemporaryExposureKeyExportFileModel> PresetTekListData(int dataVersion)
        {
            switch ((PresetTekListType)dataVersion)
            {
                case PresetTekListType.MidnightTime:
                    return new List<TemporaryExposureKeyExportFileModel> { CreateTestData(CalcMidnightTimeAddDays(-1)), CreateTestData(CalcMidnightTimeAddDays(0)) };
                case PresetTekListType.RealTime:
                    return new List<TemporaryExposureKeyExportFileModel> { CreateTestData(CalcTimeAddDays(-1)), CreateTestData(CalcTimeAddDays(0)) };
                case PresetTekListType.Nothing:
                default:
                    return new List<TemporaryExposureKeyExportFileModel>();
            }
        }

        async Task<List<TemporaryExposureKeyExportFileModel>> IHttpDataService.GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken)
        {
            /* CdnUrlBase trick for Debug_Mock
               "https://www.example.com/"(url with 2+ periods) -> download "url"+"c19r/440/list.json".  IsDownloadRequired
               "1598022036649,1598022036751,1598022036826" -> direct input timestamps.  IsDirectInput
               "https://CDN_URL_BASE/2" -> dataVersion = 2
               "https://CDN_URL_BASE/" -> dataVersion = 0 (default)
            */
            //string url = AppSettings.Instance.CdnUrlBase;
            if (mockCommonUtils.IsDownloadRequired())
            {
                // copy from GetTemporaryExposureKeyList @ ./HttpDataService.cs and delete logger part
                var container = AppSettings.Instance.BlobStorageContainerName;
                var urlJson = AppSettings.Instance.CdnUrlBase + $"{container}/{region}/list.json";
                var result = await GetCdnAsync(urlJson, cancellationToken);
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
            else if (mockCommonUtils.IsDirectInput())
            {
                Debug.WriteLine("HttpDataServiceMock::GetTemporaryExposureKeyList direct data called");
                return (mockCommonUtils.GetCreatedTimes().Select(x => CreateTestData(Convert.ToInt64(x))).ToList());
            }
            else
            {
                Debug.WriteLine("HttpDataServiceMock::GetTemporaryExposureKeyList preset data called");
                return (PresetTekListData(mockCommonUtils.GetTekListDataType()));
            }
        }


        async Task<bool> IHttpDataService.PostRegisterUserAsync()
        {
            Debug.WriteLine("HttpDataServiceMock::PostRegisterUserAsync called");
            var result = mockCommonUtils.GetRegisterDataType() switch
            {
                1 => false,
                _ => true
            };
            return await Task.FromResult(result);
        }

        Task<HttpStatusCode> IHttpDataService.PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
        {
            var code = HttpStatusCode.OK; // default. for PutSelfExposureKeys NG
            var dataType = mockCommonUtils.GetDiagnosisDataType();
            if (dataType >= 100) // HttpStatusCode >=100 by RFC2616#section-10
            {
                code = (HttpStatusCode)dataType;
            }
            else
            {
                switch (dataType)
                {
                    case 1:
                        code = HttpStatusCode.NoContent; //  for Successful PutSelfExposureKeys 
                        break;
                }
            }
            return Task.Factory.StartNew<HttpStatusCode>(() =>
            {
                Debug.WriteLine("HttpDataServiceMock::PutSelfExposureKeysAsync called");
                return code;
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
