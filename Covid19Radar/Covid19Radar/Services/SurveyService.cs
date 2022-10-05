// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using Chino;
using Covid19Radar.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Radar.Repository;
using Newtonsoft.Json.Linq;

namespace Covid19Radar.Services
{
    public interface ISurveyService
    {
        Task<SurveyContent> BuildSurveyContent(int q1, int q2, DateTime q3, bool isExposureDataProvision);
        Task<bool> SubmitSurvey(SurveyContent surveyContent);
    }

    public class SurveyService : ISurveyService
    {
        private readonly IEventLogService _eventLogService;
        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly AbsExposureNotificationApiService _exposureNotificationApiService;

        public SurveyService(
            IEventLogService eventLogService,
            IExposureDataRepository exposureDataRepository,
            AbsExposureNotificationApiService exposureNotificationApiService
            )
        {
            _eventLogService = eventLogService;
            _exposureDataRepository = exposureDataRepository;
            _exposureNotificationApiService = exposureNotificationApiService;
        }

        public async Task<SurveyContent> BuildSurveyContent(int q1, int q2, DateTime q3, bool isExposureDataProvision)
        {
            var surveyContent = new SurveyContent
            {
                Q1 = q1,
                Q2 = q2,
                Q3 = q3.ToUnixEpoch(),
                ExposureData = isExposureDataProvision ? await GetExopsureData() : null
            };
            return surveyContent;
        }

        private async Task<SurveyExposureData> GetExopsureData()
        {
            try
            {
                long enVersion = await _exposureNotificationApiService.GetVersionAsync();
                List<DailySummary> dailySummaryList = await _exposureDataRepository.GetDailySummariesAsync();
                List<ExposureWindow> exposureWindowList = await _exposureDataRepository.GetExposureWindowsAsync();

                var exposureData = new SurveyExposureData
                {
                    EnVersion = enVersion.ToString(),
                    DailySummaryList = dailySummaryList,
                    ExposureWindowList = exposureWindowList
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

