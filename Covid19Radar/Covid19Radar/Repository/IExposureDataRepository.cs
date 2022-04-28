// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Model;
using Covid19Radar.Resources;

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

        public static string ConvertToDate(DateTime utcDatetime)
            => utcDatetime.Date
                .ToLocalTime()
                .ToString("D", CultureInfo.CurrentCulture);

        public static string ConvertToTerm(DateTime utcDatetime)
        {
            var from = utcDatetime.Date.ToLocalTime();
            var to = from.AddDays(1).ToLocalTime();

            bool changeMonth = from.Month != to.Month;
            bool changeYear = from.Year != to.Year;

            string fromFormat = AppResources.ExposureDateFormatMonth;
            string toFormat = AppResources.ExposureDateFormatDate;
            if (changeMonth)
            {
                toFormat = AppResources.ExposureDateFormatMonth;
            }
            if (changeYear)
            {
                fromFormat = AppResources.ExposureDateFormatYear;
                toFormat = AppResources.ExposureDateFormatYear;
            }

            string fromStr = string.Format(fromFormat, from.Year, from.Month, from.Day, from.Hour);
            string toStr = string.Format(toFormat, to.Year, to.Month, to.Day, to.Hour);

            return string.Format("{0} {1} {2}", fromStr, AppResources.ExposuresPageTo, toStr);
        }
    }
}
