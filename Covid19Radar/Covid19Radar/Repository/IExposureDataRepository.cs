// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Model;

namespace Covid19Radar.Repository
{
    public interface IExposureDataRepository
    {
        // ExposureWindow mode
        Task<(List<DailySummary>, List<ExposureWindow>)> SetExposureDataAsync(
            List<DailySummary> dailySummaryList,
            List<ExposureWindow> exposueWindowList
            );

        Task<List<DailySummary>> GetDailySummariesAsync();
        Task<List<DailySummary>> GetDailySummariesAsync(int offsetDays);
        Task RemoveDailySummariesAsync();

        Task<List<ExposureWindow>> GetExposureWindowsAsync();
        Task<List<ExposureWindow>> GetExposureWindowsAsync(int offsetDays);
        Task RemoveExposureWindowsAsync();

        // Legacy v1 mode
        List<UserExposureInfo> GetExposureInformationList();
        List<UserExposureInfo> GetExposureInformationList(int offsetDays);

        void SetExposureInformation(List<UserExposureInfo> informationList);
        bool AppendExposureData(
            ExposureSummary exposureSummary,
            List<ExposureInformation> exposureInformationList,
            int minimumRiskScore
            );

        void RemoveExposureInformation();
        void RemoveOutOfDateExposureInformation(int offsetDays);
        int GetV1ExposureCount(int offsetDays);
    }
}
