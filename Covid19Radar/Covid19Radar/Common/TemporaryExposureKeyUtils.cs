﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chino;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Common
{
    public static class TemporaryExposureKeyUtils
    {
        public static IList<TemporaryExposureKey> FiilterTemporaryExposureKeys(
            IList<TemporaryExposureKey> temporaryExposureKeys,
            DateTime diagnosisDate,
            int daysToSendTek,
            ILoggerService loggerService
            )
        {
            loggerService.StartMethod();

            List<TemporaryExposureKey> filteredTemporaryExposureKeys = new List<TemporaryExposureKey>();

            try
            {
                var fromDateTime = diagnosisDate.AddDays(daysToSendTek);
                var fromInterval = fromDateTime.ToEnInterval();

                filteredTemporaryExposureKeys.AddRange(temporaryExposureKeys.Where(x => x.RollingStartIntervalNumber >= fromInterval));

                loggerService.Info($"Filter: After {fromInterval}");
            }
            catch (Exception ex)
            {
                loggerService.Exception("Temporary exposure keys filtering failed", ex);
                throw ex;
            }
            finally
            {
                loggerService.Info($"Count: {filteredTemporaryExposureKeys.Count()}");
                loggerService.EndMethod();
            }

            return filteredTemporaryExposureKeys;
        }
    }
}
