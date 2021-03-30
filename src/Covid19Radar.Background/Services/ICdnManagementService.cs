/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public interface ICdnManagementService
    {
        Task PurgeAsync(IList<string> contentPaths);
    }
}
