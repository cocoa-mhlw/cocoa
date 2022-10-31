// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using Covid19Radar.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Radar.Repository;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Covid19Radar.Services
{
    public interface ISurveyService
    {
        Task<SurveyContent> BuildSurveyContent(int q1, int q2, bool isAppStartDate, bool isExposureDataProvision);
        Task<bool> SubmitSurvey(SurveyContent surveyContent);
    }

    public class SurveyService : ISurveyService
    {
        private const long FromDateMillisEpoch = 1649289600000; // 2022/04/07 00:00:00 UTC

        private readonly IEventLogService _eventLogService;
        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly IUserDataRepository _userDataRepository;

        public SurveyService(
            IEventLogService eventLogService,
            IExposureDataRepository exposureDataRepository,
            IUserDataRepository userDataRepository
            )
        {
            _eventLogService = eventLogService;
            _exposureDataRepository = exposureDataRepository;
            _userDataRepository = userDataRepository;
        }

        public async Task<SurveyContent> BuildSurveyContent(int q1, int q2, bool isAppStartDate, bool isExposureDataProvision)
        {
            var surveyContent = new SurveyContent
            {
                Q1 = q1 > 0 ? (int?)q1 : null,
                Q2 = q2 > 0 ? (int?)q2 : null,
                StartDate = isAppStartDate ? (long?)_userDataRepository.GetStartDate().ToUnixEpoch() : null,
                ExposureData = isExposureDataProvision ? await GetExopsureData() : null
            };
            return surveyContent;
        }

        private async Task<SurveyExposureData> GetExopsureData()
        {
            try
            {
                V1ExposureRiskCalculationConfiguration riskConfiguration
                    = ExposureRiskCalculationConfigurationRepository.CreateDefaultConfiguration();

                List<Chino.DailySummary> chinoDailySummaryList
                    = await _exposureDataRepository.GetDailySummariesAsync();

                List<SurveyExposureData.DailySummary> dailySummaryList
                    = chinoDailySummaryList
                    .Where(item => item.DateMillisSinceEpoch >= FromDateMillisEpoch)
                    .Select(
                        item => new SurveyExposureData.DailySummary
                        {
                            DateMillisSinceEpoch = item.DateMillisSinceEpoch,
                            ExposureDetected = riskConfiguration.DailySummary_DaySummary_ScoreSum.Cond(item.DaySummary.MaximumScore) ? 1 : 0
                        })
                    .ToList();

                var exposureData = new SurveyExposureData
                {
                    DailySummaryList = dailySummaryList,
                };

                return exposureData;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> SubmitSurvey(SurveyContent surveyContent)
        {
            var eventLog = new EventLog
            {
                HasConsent = true,
                Epoch = DateTime.UtcNow.ToUnixEpoch(),
                Type = EventType.Survey.Type,
                Subtype = EventType.Survey.SubType,
                Content = JObject.FromObject(surveyContent),
            };

            return await _eventLogService.SendAsync(eventLog);
        }
    }
}

