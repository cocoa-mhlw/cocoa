/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using Covid19Radar.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{
    public interface IV1DeviceValidationService
    {
        Task<bool> Validation(V1DiagnosisSubmissionParameter param, DateTimeOffset requestTime);
    }
}
