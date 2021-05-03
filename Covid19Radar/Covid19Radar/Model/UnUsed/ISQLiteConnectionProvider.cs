﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Model
{
    public interface ISQLiteConnectionProvider
    {
        SQLiteConnection GetConnection();
    }
}
