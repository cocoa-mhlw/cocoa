/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Covid19Radar.Services
{
    class HttpDataServiceMock : IHttpDataService
    {
        private readonly HttpClient downloadClient;
        private readonly MockCommonUtils mockCommonUtils = new MockCommonUtils();

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
            // set 0 hour,1 min,2 sec,3 millisecond for debug
            return new DateTimeOffset(new DateTime(d.Year, d.Month, d.Day, 0, 1, 2, 3)).ToUnixTimeMilliseconds();
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

        async Task<HttpStatusCode> IHttpDataService.PostRegisterUserAsync()
        {
            Debug.WriteLine("HttpDataServiceMock::PostRegisterUserAsync called");
            var code = HttpStatusCode.OK;
            var result = mockCommonUtils.GetRegisterDataType();
            if (result >= 100)
            {
                code = (HttpStatusCode)result;
            }
            else
            {
                switch (result)
                {
                    case 1:
                        code = HttpStatusCode.NoContent;
                        break;
                }
            }

            return await Task.FromResult(code);
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

            Debug.WriteLine("HttpDataServiceMock::PutSelfExposureKeysAsync called");
            return Task.FromResult(code);
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

    public class MockCommonUtils
    {
        public string CdnUrlBase => AppSettings.Instance.CdnUrlBase;
        public string ApiUrlBase => AppSettings.Instance.ApiUrlBase;

        public bool IsDownloadRequired()
                => Regex.IsMatch(CdnUrlBase, @"^https://.*\..*\..*/$");

        public bool IsDirectInput()
        => Regex.IsMatch(CdnUrlBase, @"^(\d+,)+\d+,*$");


        private ushort NumberEndofSentence(string url)
        {
            Match match = Regex.Match(url, @"(?<d>\d+)$");
            ushort number = 0;
            if (match.Success)
            {
                number = Convert.ToUInt16(match.Groups["d"].Value);
            }
            return number;
        }
        public List<string> GetCreatedTimes()
            => CdnUrlBase.Split(",").ToList();
        public ushort GetTekListDataType()
        => NumberEndofSentence(CdnUrlBase);
        public string[] GetApiUrlSegment()
        {
            // "url/api" -> { "url/api", "", "" }
            // "url/base/api/register1/diagnosis2" -> { "url/base/api", "/register1", "/diagnosis2" } 
            // "url/api1/r1/d2" -> { "url/api1", "/r1", "/d2" } 
            // "url/api1/d2/r1" -> { "url/api1", "/r1", "/d2" } 
            var url = ApiUrlBase;
            var r = new Regex("/r(egister)?[0-9]+");
            var d = new Regex("/d(iagnosis)?[0-9]+");
            var urlRegister = r.Match(url).Value;
            url = r.Replace(url, "");
            var urlDiagnosis = d.Match(url).Value;
            url = d.Replace(url, "");
            var urlApi = url;
            return new string[] { urlApi, urlRegister, urlDiagnosis };
        }
        public ushort GetDiagnosisDataType()
            => NumberEndofSentence(GetApiUrlSegment()[2]);
        public ushort GetRegisterDataType()
            => NumberEndofSentence(GetApiUrlSegment()[1]);
        public ushort GetApiDataType()
            => NumberEndofSentence(GetApiUrlSegment()[0]);
        public bool IsDirectInputApi()
            => Regex.IsMatch(GetApiUrlSegment()[0], @"^(\d+,)+\d+,?$");
        public List<string> GetApiStrings()
            => GetApiUrlSegment()[0].Split(",").ToList();
    }
}
