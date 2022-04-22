﻿/*This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Covid19Radar.Api.Models;

namespace Covid19Radar.Api.DataAccess
{
    public interface IEventLogRepository
    {
        Task UpsertAsync(EventLogModel eventLog);
    }
}
