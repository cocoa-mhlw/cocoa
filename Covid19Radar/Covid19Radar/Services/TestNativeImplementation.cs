/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;

namespace Covid19Radar.Services
{
    public class MockCommonUtils
    {
        public string CdnUrlBase { get => AppSettings.Instance.CdnUrlBase; }
        public string ApiUrlBase { get => AppSettings.Instance.ApiUrlBase; }

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
            return (number);
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
            return (new string[] { urlApi, urlRegister, urlDiagnosis });
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
    public class TestNativeImplementation : INativeImplementation
    {
        static readonly Random random = new Random();
        private readonly MockCommonUtils mockCommonUtils;

        Task WaitRandom()
            => Task.Delay(random.Next(100, 2500));

        public async Task StartAsync()
        {
            await WaitRandom();
            Preferences.Set("fake_enabled", true);
        }

        public async Task StopAsync()
        {
            await WaitRandom();
            Preferences.Set("fake_enabled", false);
        }

        public async Task<bool> IsEnabledAsync()
        {
            await WaitRandom();
            return Preferences.Get("fake_enabled", true);
        }

        public async Task<IEnumerable<TemporaryExposureKey>> GetSelfTemporaryExposureKeysAsync()
        {
            var keys = new List<TemporaryExposureKey>();

            for (var i = 1; i < 14; i++)
                keys.Add(GenerateRandomKey(i));

            await WaitRandom();

            return keys;
        }

        public Task<Status> GetStatusAsync()
            => Task.FromResult(Preferences.Get("fake_enabled", true) ? Status.Active : Status.Disabled);

        enum PresetDataType
        {
            TwoLowRiskMatches = 0, // two low-risk matches (default for v1.2.3)
            OneHighRiskMatchAnd2LowRiskMatches = 1, // one high-risk match and 2 low-risk matches 
            NoMatch = 2,
            // please add "YourDataType = <int>"
        }

        private ushort[] DataPreset(int dataType)
        {
            /* DataPreset returns ushort[];
               index[0] ~ index[4] : data for ExposureDetectionSummary 
               index[5] ~ index[10] : 1st data for ExposureInfo
               index[11] ~ index[15] : 2nd data for ExposureInfo
               index[16] ~ index[20] : 3rd data for ExposureInfo
               ....
             */
            switch ((PresetDataType)dataType)
            {
                case PresetDataType.OneHighRiskMatchAnd2LowRiskMatches:
                    return (
                        new ushort[] {10, 3, 27, 0, 0, // ExposureDetectionSummary 
                                       13, 15, 65, 27, (ushort)RiskLevel.High, // ExposureInfo 1st
                                       10, 15, 65, 5, (ushort)RiskLevel.Medium, // ExposureInfo 2st
                                       11,  5, 40, 3, (ushort)RiskLevel.Low, // ExposureInfo  3nd
                        });
                case PresetDataType.NoMatch:
                    return (
                        new ushort[] {0, 0, 0, 0, 0, // ExposureDetectionSummary
                        });
                case PresetDataType.TwoLowRiskMatches:
                default:
                    return (
                        new ushort[] {10, 2, 5, 0, 0, // ExposureDetectionSummary
                                       10, 15, 65, 5, (ushort)RiskLevel.Medium, // ExposureInfo 1st (RiskLevel.Medium=4)
                                       11,  5, 40, 3, (ushort)RiskLevel.Low, // ExposureInfo  2nd(RiskLevel.Low=2)
                        });
            }
        }

        private ushort[] CreatePresetData()
        {
            if (mockCommonUtils.IsDirectInputApi())
            {
                return mockCommonUtils.GetApiStrings().Select(x => Convert.ToUInt16(x)).ToArray();
            }
            return DataPreset(mockCommonUtils.GetApiDataType());

        }

        public Task<(ExposureDetectionSummary summary, Func<Task<IEnumerable<ExposureInfo>>> getInfo)> DetectExposuresAsync(IEnumerable<string> files)
        {
            /* ApiUrlBase trick for Debug_Mock
            "10,2,5,0,0,10,15,65,5,4,11,5,40,3,2" -> direct input (the same with default)
            "https://API_URL_BASE/api2" -> dataVer = 2
            "https://API_URL_BASE/api" -> dataVer = 0 (default)
            others -> dataVer is the number at the end of the sentence
            */
            var dataPreset = CreatePresetData();
            int index = 0;
            var summary = new ExposureDetectionSummary(dataPreset[index++], dataPreset[index++],
                dataPreset[index++], new TimeSpan[dataPreset[index++]], dataPreset[index++]);
            // c.f.: ExposureDetectionSummary(daysSinceLastExposure=dataPreset[0],matchedKeyCount=dataPreset[1],maximumRiskScore=dataPreset[2],attenuationDurations=new TimeSpan[dataPreset[3]],summationRiskScore=dataPreset[4])

            Task<IEnumerable<ExposureInfo>> GetInfo()
            {
                var info = new List<ExposureInfo>();
                while (index < dataPreset.Length)
                {
                    info.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-dataPreset[index++]),
                        TimeSpan.FromMinutes(dataPreset[index++]), dataPreset[index++],
                        dataPreset[index++], (Xamarin.ExposureNotifications.RiskLevel)dataPreset[index++]));
                    // c.f.: ExposureInfo(DateTime timestamp, TimeSpan duration, int attenuationValue, int totalRiskScore, RiskLevel riskLevel)
                };
                return Task.FromResult<IEnumerable<ExposureInfo>>(info);
            }

            return Task.FromResult<(ExposureDetectionSummary, Func<Task<IEnumerable<ExposureInfo>>>)>((summary, GetInfo));
        }

        static TemporaryExposureKey GenerateRandomKey(int daysAgo)
        {
            var buffer = new byte[16];
            random.NextBytes(buffer);

            return new TemporaryExposureKey(
                buffer,
                DateTimeOffset.UtcNow.AddDays(-1 * daysAgo),
                TimeSpan.FromMinutes(random.Next(5, 120)),
                (RiskLevel)random.Next(1, 8));
        }
    }
}
