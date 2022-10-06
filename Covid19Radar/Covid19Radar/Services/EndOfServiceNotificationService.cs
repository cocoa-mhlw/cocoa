// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public interface IEndOfServiceNotificationService
    {
        Task ShowNotificationAsync(CancellationTokenSource cancellationTokenSource = null);
    }

    public class EndOfServiceNotificationService : IEndOfServiceNotificationService
    {
        private readonly ILoggerService _loggerService;
        private readonly ILocalNotificationService _localNotificationService;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly IUserDataRepository _userDataRepository;

        private readonly int MaxNotificationCount = 2;
        private readonly int[] ScheduleDays = new int[] { 3, 4 };

        public EndOfServiceNotificationService(
            ILoggerService loggerService,
            ILocalNotificationService localNotificationService,
            IDateTimeUtility dateTimeUtility,
            IUserDataRepository userDataRepository
            )
        {
            _loggerService = loggerService;
            _localNotificationService = localNotificationService;
            _dateTimeUtility = dateTimeUtility;
            _userDataRepository = userDataRepository;
        }

        public async Task ShowNotificationAsync(CancellationTokenSource cancellationTokenSource = null)
        {
            _loggerService.StartMethod();

            try
            {
                DateTime utcNow = _dateTimeUtility.UtcNow;
                DateTime jstNow = _dateTimeUtility.JstNow;

                if (!_userDataRepository.IsAllAgreed())
                {
                    _loggerService.Info("No notification (Is not agreed)");
                    return;
                }

                DateTime endDateUtc = AppConstants.SURVEY_END_DATE_UTC;
                if (endDateUtc.CompareTo(utcNow) <= 0)
                {
                    _loggerService.Info("No notification (Survey end)");
                    return;
                }

                int endOfServiceNotificationCount = _userDataRepository.GetEndOfServiceNotificationCount();
                if (endOfServiceNotificationCount > MaxNotificationCount - 1)
                {
                    _loggerService.Info("No notification (Max notifications)");
                    return;
                }

                DateTime? nextSchedule = _userDataRepository.GetEndOfServiceNotificationNextSchedule();
                if (nextSchedule is null)
                {
                    nextSchedule = GetNextSchedule(ScheduleDays[endOfServiceNotificationCount], utcNow);
                    _userDataRepository.SetEndOfServiceNotificationNextSchedule((DateTime)nextSchedule);
                    _loggerService.Info($"No notification (First schedule) {nextSchedule}");
                    return;
                }

                if (((DateTime)nextSchedule).CompareTo(utcNow) > 0)
                {
                    _loggerService.Info("No notification (Before schedule)");
                    return;
                }

                if (jstNow.Hour < 9 || jstNow.Hour >= 21)
                {
                    _loggerService.Info("No notification (Off hours)");
                    return;
                }
                
                endOfServiceNotificationCount++;
                _userDataRepository.SetEndOfServiceNotificationCount(endOfServiceNotificationCount);

                if (endOfServiceNotificationCount <= MaxNotificationCount - 1)
                {
                    nextSchedule = GetNextSchedule(ScheduleDays[endOfServiceNotificationCount], (DateTime)nextSchedule);
                    _userDataRepository.SetEndOfServiceNotificationNextSchedule((DateTime)nextSchedule);
                    _loggerService.Info($"Set next schedule. {nextSchedule}");
                }

                _loggerService.Info("Show notification.");
                await _localNotificationService.ShowEndOfServiceNoticationAsync();
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        private DateTime GetNextSchedule(int days, DateTime baseDateTimeUtc)
        {
            DateTime baseDateTimeJst = TimeZoneInfo.ConvertTimeFromUtc(baseDateTimeUtc, AppConstants.TIMEZONE_JST);
            DateTime nextScheduleDate = baseDateTimeJst.Date.AddDays(days);

            var random = new Random();
            int seconds = random.Next(9 * 60 * 60, 21 * 60 * 60 - 1);

            DateTime nexeScheculeDateTime = nextScheduleDate.AddSeconds(seconds);

            return TimeZoneInfo.ConvertTimeToUtc(nexeScheculeDateTime, AppConstants.TIMEZONE_JST);
        }
    }
}

