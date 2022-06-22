// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public class ExposureDataRepository : IExposureDataRepository
    {
        private const string EMPTY_LIST_JSON = "[]";

        private readonly ISecureStorageService _secureStorageService;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly ILoggerService _loggerService;

        public ExposureDataRepository(
            ISecureStorageService secureStorageService,
            IDateTimeUtility dateTimeUtility,
            ILoggerService loggerService
            )
        {
            _secureStorageService = secureStorageService;
            _dateTimeUtility = dateTimeUtility;
            _loggerService = loggerService;
        }

        #region ExposureWindow mode
        private readonly DailySummary.Comparer _dailySummaryComparer = new DailySummary.Comparer();
        private readonly ExposureWindow.Comparer _exposureWindowComparer = new ExposureWindow.Comparer();

        public async Task<(List<DailySummary>, List<ExposureWindow>)> SetExposureDataAsync(
            List<DailySummary> dailySummaryList,
            List<ExposureWindow> exposueWindowList
            )
        {
            _loggerService.StartMethod();

            List<DailySummary> existDailySummaryList = await GetDailySummariesAsync();
            List<ExposureWindow> existExposureWindowList = await GetExposureWindowsAsync();

            List<DailySummary> filteredExistDailySummaryList = new List<DailySummary>();

            // Filter and merge DailySummaries that have same DateMillisSinceEpoch value.
            foreach (var existDailySummary in existDailySummaryList)
            {
                var conflictDailySummaryList = dailySummaryList
                    .Where(ds => ds.DateMillisSinceEpoch == existDailySummary.DateMillisSinceEpoch)
                    .ToList();

                var conflictDailySummaryListCount = conflictDailySummaryList.Count();
                if (conflictDailySummaryListCount == 0)
                {
                    filteredExistDailySummaryList.Add(existDailySummary);
                    continue;
                }
                else if (conflictDailySummaryListCount > 1)
                {
                    _loggerService.Warning($"The list conflictDailySummaryList count should be 1 but {conflictDailySummaryListCount}." +
                        "conflictDailySummaryList will be sorted and selected first value.");
                    conflictDailySummaryList.Sort(_dailySummaryComparer);
                }

                // `conflictDailySummaryList` count must be 1,
                // because the DailySummary objects that have same DateMillisSinceEpoch value must be saved after merge.
                DailySummary newDailySummary = conflictDailySummaryList.First();

                if (existDailySummary.Equals(newDailySummary))
                {
                    filteredExistDailySummaryList.Add(existDailySummary);
                }
                else
                {
                    MergeDailySummarySelectMaxValues(existDailySummary, newDailySummary);
                }
            }

            List<DailySummary> unionDailySummaryList = filteredExistDailySummaryList.Union(dailySummaryList).ToList();
            List<ExposureWindow> unionExposureWindowList = existExposureWindowList.Union(exposueWindowList).ToList();
            unionDailySummaryList.Sort(_dailySummaryComparer);
            unionExposureWindowList.Sort(_exposureWindowComparer);

            await SaveExposureDataAsync(unionDailySummaryList, unionExposureWindowList);

            List<DailySummary> newDailySummaryList = unionDailySummaryList.Except(filteredExistDailySummaryList).ToList();
            List<ExposureWindow> newExposureWindowList = unionExposureWindowList.Except(existExposureWindowList).ToList();

            _loggerService.EndMethod();

            return (newDailySummaryList, newExposureWindowList);
        }

        /// <summary>
        /// Select and merge the maximum values of each DailySummary objects.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void MergeDailySummarySelectMaxValues(DailySummary from, DailySummary to)
        {
            _loggerService.StartMethod();

            if (from.DateMillisSinceEpoch != to.DateMillisSinceEpoch)
            {
                _loggerService.Info($"DateMillisSinceEpoch is not match: {from.DateMillisSinceEpoch}, {to.DateMillisSinceEpoch}");
                return;
            }

            if (from.DaySummary != null)
            {
                if (to.DaySummary == null)
                {
                    to.DaySummary = new ExposureSummaryData();
                }
                to.DaySummary.ScoreSum = Math.Max(from.DaySummary.ScoreSum, to.DaySummary.ScoreSum);
                to.DaySummary.MaximumScore = Math.Max(from.DaySummary.MaximumScore, to.DaySummary.MaximumScore);
                to.DaySummary.WeightedDurationSum = Math.Max(from.DaySummary.WeightedDurationSum, to.DaySummary.WeightedDurationSum);
            }

            if (from.ConfirmedTestSummary != null)
            {
                if (to.ConfirmedTestSummary == null)
                {
                    to.ConfirmedTestSummary = new ExposureSummaryData();
                }
                to.ConfirmedTestSummary.ScoreSum = Math.Max(from.ConfirmedTestSummary.ScoreSum, to.ConfirmedTestSummary.ScoreSum);
                to.ConfirmedTestSummary.MaximumScore = Math.Max(from.ConfirmedTestSummary.MaximumScore, to.ConfirmedTestSummary.MaximumScore);
                to.ConfirmedTestSummary.WeightedDurationSum = Math.Max(from.ConfirmedTestSummary.WeightedDurationSum, to.ConfirmedTestSummary.WeightedDurationSum);
            }

            if (from.ConfirmedClinicalDiagnosisSummary != null)
            {
                if (to.ConfirmedClinicalDiagnosisSummary == null)
                {
                    to.ConfirmedClinicalDiagnosisSummary = new ExposureSummaryData();
                }
                to.ConfirmedClinicalDiagnosisSummary.ScoreSum = Math.Max(from.ConfirmedClinicalDiagnosisSummary.ScoreSum, to.ConfirmedClinicalDiagnosisSummary.ScoreSum);
                to.ConfirmedClinicalDiagnosisSummary.MaximumScore = Math.Max(from.ConfirmedClinicalDiagnosisSummary.MaximumScore, to.ConfirmedClinicalDiagnosisSummary.MaximumScore);
                to.ConfirmedClinicalDiagnosisSummary.WeightedDurationSum = Math.Max(from.ConfirmedClinicalDiagnosisSummary.WeightedDurationSum, to.ConfirmedClinicalDiagnosisSummary.WeightedDurationSum);
            }

            if (from.SelfReportedSummary != null)
            {
                if (to.SelfReportedSummary == null)
                {
                    to.SelfReportedSummary = new ExposureSummaryData();
                }
                to.SelfReportedSummary.ScoreSum = Math.Max(from.SelfReportedSummary.ScoreSum, to.SelfReportedSummary.ScoreSum);
                to.SelfReportedSummary.MaximumScore = Math.Max(from.SelfReportedSummary.MaximumScore, to.SelfReportedSummary.MaximumScore);
                to.SelfReportedSummary.WeightedDurationSum = Math.Max(from.SelfReportedSummary.WeightedDurationSum, to.SelfReportedSummary.WeightedDurationSum);
            }

            if (from.RecursiveSummary != null)
            {
                if (to.RecursiveSummary == null)
                {
                    to.RecursiveSummary = new ExposureSummaryData();
                }
                to.RecursiveSummary.ScoreSum = Math.Max(from.RecursiveSummary.ScoreSum, to.RecursiveSummary.ScoreSum);
                to.RecursiveSummary.MaximumScore = Math.Max(from.RecursiveSummary.MaximumScore, to.RecursiveSummary.MaximumScore);
                to.RecursiveSummary.WeightedDurationSum = Math.Max(from.RecursiveSummary.WeightedDurationSum, to.RecursiveSummary.WeightedDurationSum);
            }

            _loggerService.EndMethod();
        }

        private Task SaveExposureDataAsync(IList<DailySummary> dailySummaryList, IList<ExposureWindow> exposureWindowList)
        {
            _loggerService.StartMethod();

            string dailySummaryListJson = JsonConvert.SerializeObject(dailySummaryList);
            string exposureWindowListJson = JsonConvert.SerializeObject(exposureWindowList);

            try
            {
                _secureStorageService.SetStringValue(PreferenceKey.DailySummaries, dailySummaryListJson);
                _secureStorageService.SetStringValue(PreferenceKey.ExposureWindows, exposureWindowListJson);
                return Task.CompletedTask;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public Task<List<DailySummary>> GetDailySummariesAsync()
        {
            _loggerService.StartMethod();

            try
            {
                string dailySummariesJson = _secureStorageService.GetStringValue(PreferenceKey.DailySummaries, EMPTY_LIST_JSON);
                return Task.FromResult(
                    JsonConvert.DeserializeObject<List<DailySummary>>(dailySummariesJson)
                );
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task<List<DailySummary>> GetDailySummariesAsync(int offsetDays)
        {
            return (await GetDailySummariesAsync())
                .Where(dailySummary => dailySummary.GetDateTime().CompareTo(_dateTimeUtility.UtcNow.AddDays(offsetDays)) >= 0)
                .ToList();
        }

        public Task<List<ExposureWindow>> GetExposureWindowsAsync()
        {
            _loggerService.StartMethod();

            try
            {
                string exposureWindowListJson = _secureStorageService.GetStringValue(PreferenceKey.ExposureWindows, EMPTY_LIST_JSON);
                return Task.FromResult(
                    JsonConvert.DeserializeObject<List<ExposureWindow>>(exposureWindowListJson)
                );
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task<List<ExposureWindow>> GetExposureWindowsAsync(int offsetDays)
        {
            return (await GetExposureWindowsAsync())
                .Where(dailySummary => dailySummary.GetDateTime().CompareTo(_dateTimeUtility.UtcNow.AddDays(offsetDays)) >= 0)
                .ToList();
        }

        public Task RemoveDailySummariesAsync()
        {
            _loggerService.StartMethod();

            try
            {
                _secureStorageService.RemoveValue(PreferenceKey.DailySummaries);
                return Task.CompletedTask;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public Task RemoveExposureWindowsAsync()
        {
            _loggerService.StartMethod();

            try
            {
                _secureStorageService.RemoveValue(PreferenceKey.ExposureWindows);
                return Task.CompletedTask;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
        #endregion

        #region Legacy v1
        public List<UserExposureInfo> GetExposureInformationList()
        {
            _loggerService.StartMethod();
            List<UserExposureInfo> result = null;
            var exposureInformationJson = _secureStorageService.GetStringValue(PreferenceKey.ExposureInformation, null);
            if (!string.IsNullOrEmpty(exposureInformationJson))
            {
                result = JsonConvert.DeserializeObject<List<UserExposureInfo>>(exposureInformationJson);
            }
            _loggerService.EndMethod();
            return result ?? new List<UserExposureInfo>();
        }

        public List<UserExposureInfo> GetExposureInformationList(int offsetDays)
        {
            _loggerService.StartMethod();
            var date = _dateTimeUtility.UtcNow.AddDays(offsetDays);
            var list = GetExposureInformationList()?
                .Where(x => x.Timestamp.CompareTo(date) >= 0)
                .ToList();
            _loggerService.EndMethod();
            return list;
        }

        public void SetExposureInformation(List<UserExposureInfo> exposureInformationList)
        {
            _loggerService.StartMethod();

            string exposureInformationListJson = JsonConvert.SerializeObject(exposureInformationList);

            try
            {
                _secureStorageService.SetStringValue(PreferenceKey.ExposureInformation, exposureInformationListJson);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public bool AppendExposureData(
            ExposureSummary exposureSummary,
            List<ExposureInformation> exposureInformationList,
            int minimumRiskScore
            )
        {
            var existExposureInformationList = GetExposureInformationList() ?? new List<UserExposureInfo>();
            bool isNewExposureDetected = false;

            if (exposureSummary.MaximumRiskScore >= minimumRiskScore)
            {
                foreach (var exposureInfo in exposureInformationList)
                {
                    _loggerService.Info($"Exposure.Timestamp: {exposureInfo.DateMillisSinceEpoch}");
                    _loggerService.Info($"Exposure.Duration: {exposureInfo.DurationInMillis}");
                    _loggerService.Info($"Exposure.AttenuationValue: {exposureInfo.AttenuationValue}");
                    _loggerService.Info($"Exposure.TotalRiskScore: {exposureInfo.TotalRiskScore}");
                    _loggerService.Info($"Exposure.TransmissionRiskLevel: {exposureInfo.TransmissionRiskLevel}");

                    if (exposureInfo.TotalRiskScore >= minimumRiskScore)
                    {
                        existExposureInformationList.Add(new UserExposureInfo(exposureInfo));
                        isNewExposureDetected = true;
                    }
                }

                _loggerService.Info($"Save ExposureSummary. MatchedKeyCount: {exposureSummary.MatchedKeyCount}");
                _loggerService.Info($"Save ExposureInformation. Count: {existExposureInformationList.Count}");

                existExposureInformationList.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));

                SetExposureInformation(existExposureInformationList);
            }

            return isNewExposureDetected;
        }

        public void RemoveExposureInformation()
        {
            _loggerService.StartMethod();
            _secureStorageService.RemoveValue(PreferenceKey.ExposureInformation);
            _loggerService.EndMethod();
        }

        public void RemoveOutOfDateExposureInformation(int offsetDays)
        {
            _loggerService.StartMethod();

            var informationList = GetExposureInformationList(offsetDays);
            SetExposureInformation(informationList);

            _loggerService.EndMethod();
        }
    }
}
#endregion
