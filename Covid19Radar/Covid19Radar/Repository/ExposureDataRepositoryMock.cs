// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Model;

namespace Covid19Radar.Repository
{
    public class ExposureDataRepositoryMock : IExposureDataRepository
    {

        //private List<DailySummary> dummyDailySummaries = new List<DailySummary>()
        //{
        //};

        //private List<ExposureWindow> dummyExposureWindows = new List<ExposureWindow>()
        //{
        //};

        //private List<UserExposureInfo> dummyUserExposureInfos = new List<UserExposureInfo>()
        //{
        //};

        private List<DailySummary> dummyDailySummaries = new List<DailySummary>()
        {
            new DailySummary()
            {
                DateMillisSinceEpoch = DateTime.SpecifyKind(new DateTime(2022, 1, 10), DateTimeKind.Utc).ToUnixEpoch() * 1000,
                DaySummary = new ExposureSummaryData()
                {
                    ScoreSum = 2000.0
                }
            },
            new DailySummary()
            {
                DateMillisSinceEpoch = DateTime.SpecifyKind(new DateTime(2022, 1, 11), DateTimeKind.Utc).ToUnixEpoch() * 1000,
                DaySummary = new ExposureSummaryData()
                {
                    ScoreSum = 1999.0
                }
            },
        };

        private List<ExposureWindow> dummyExposureWindows = new List<ExposureWindow>()
        {
            new ExposureWindow()
            {
                DateMillisSinceEpoch = DateTime.SpecifyKind(new DateTime(2022, 1, 10), DateTimeKind.Utc).ToUnixEpoch() * 1000,
                ScanInstances = new List<ScanInstance>()
                {
                    new ScanInstance()
                    {
                        SecondsSinceLastScan = 200,
                    },
                    new ScanInstance()
                    {
                        SecondsSinceLastScan = 1800,
                    },
                }
            },
            new ExposureWindow()
            {
                DateMillisSinceEpoch = DateTime.SpecifyKind(new DateTime(2022, 1, 10), DateTimeKind.Utc).ToUnixEpoch() * 1000,
                ScanInstances = new List<ScanInstance>()
                {
                    new ScanInstance()
                    {
                        SecondsSinceLastScan = 1800,
                    },
                }
            },
            new ExposureWindow()
            {
                DateMillisSinceEpoch = DateTime.SpecifyKind(new DateTime(2022, 1, 11), DateTimeKind.Utc).ToUnixEpoch() * 1000,
                ScanInstances = new List<ScanInstance>()
                {
                    new ScanInstance()
                    {
                        SecondsSinceLastScan = 1800,
                    },
                    new ScanInstance()
                    {
                        SecondsSinceLastScan = 1800,
                    },
                    new ScanInstance()
                    {
                        SecondsSinceLastScan = 1800,
                    },
                }
            },
        };

        private List<UserExposureInfo> dummyUserExposureInfos = new List<UserExposureInfo>()
        {
            new UserExposureInfo()
            {
                Timestamp = DateTime.SpecifyKind(new DateTime(2022, 1, 1), DateTimeKind.Utc)
            }
        };

        public Task<List<DailySummary>> GetDailySummariesAsync()
        {
            return Task.FromResult(dummyDailySummaries);
        }

        public Task<List<ExposureWindow>> GetExposureWindowsAsync()
        {
            return Task.FromResult(dummyExposureWindows);
        }

        public async Task<List<DailySummary>> GetDailySummariesAsync(int offsetDays)
            => await GetDailySummariesAsync();

        public async Task<List<ExposureWindow>> GetExposureWindowsAsync(int offsetDays)
            => await GetExposureWindowsAsync();

        public int GetV1ExposureCount(int offsetDays)
        {
            throw new NotImplementedException();
        }

        public List<UserExposureInfo> GetExposureInformationList()
            => dummyUserExposureInfos;

        public List<UserExposureInfo> GetExposureInformationList(int offsetDays)
            => GetExposureInformationList();

        #region Do nothing
        public Task RemoveDailySummariesAsync()
        {
            // Do nothing

            return Task.CompletedTask;
        }

        public void RemoveExposureInformation()
        {
            // Do nothing
        }

        public Task RemoveExposureWindowsAsync()
        {
            // Do nothing

            return Task.CompletedTask;
        }

        public void RemoveOutOfDateExposureInformation(int offsetDays)
        {
            // Do nothing
        }

        public Task<(List<DailySummary>, List<ExposureWindow>)> SetExposureDataAsync(List<DailySummary> dailySummaryList, List<ExposureWindow> exposueWindowList)
        {
            // Do nothing
            return Task.FromResult((new List<DailySummary>(), new List<ExposureWindow>()));
        }

        public void SetExposureInformation(List<UserExposureInfo> informationList)
        {
            // Do nothing
        }

        public bool AppendExposureData(ExposureSummary exposureSummary, List<ExposureInformation> exposureInformationList, int minimumRiskScore)
        {
            // Do nothing
            return true;
        }
        #endregion
    }
}
