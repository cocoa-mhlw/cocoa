// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Radar.Model;

namespace Covid19Radar.Services
{
    public class EventLogServiceNop : IEventLogService
    {
        public Task<List<EventLog>> SendAllAsync(
            long maxSize,
            int maxRetry
            )
        {
            // do nothing
            return Task.FromResult(new List<EventLog>());
        }
    }
}
