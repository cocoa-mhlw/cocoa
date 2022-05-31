// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Model;

namespace Covid19Radar.Services
{
    public class EventLogServiceNop : IEventLogService
    {
        public Task SendExposureDataAsync(
            string idempotencyKey,
            ExposureConfiguration exposureConfiguration,
            string deviceModel, string enVersion,
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformation
            )
        {
            // do nothing
            return Task.CompletedTask;
        }

        public Task SendExposureDataAsync(
            string idempotencyKey,
            ExposureConfiguration exposureConfiguration,
            string deviceModel, string enVersion,
            IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows
            )
        {
            // do nothing
            return Task.CompletedTask;
        }

        public Task SendExposureDataAsync(
            string idempotencyKey,
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion
            )
        {
            // do nothing
            return Task.CompletedTask;
        }

        public Task SendAllAsync(
            long maxSize,
            int maxRetry
            )
        {
            // do nothing
            return Task.CompletedTask;
        }

        public Task SendAsync(
            string idempotencyKey,
            List<EventLog> eventLogList
            )
        {
            // do nothing
            return Task.CompletedTask;
        }
    }
}
