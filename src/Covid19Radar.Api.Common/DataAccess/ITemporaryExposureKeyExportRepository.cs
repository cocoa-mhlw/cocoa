﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Models;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public interface ITemporaryExposureKeyExportRepository
    {
        Task<TemporaryExposureKeyExportModel> CreateAsync(TemporaryExposureKeyExportModel model);
        Task<TemporaryExposureKeyExportModel> GetAsync(string id);
        Task UpdateAsync(TemporaryExposureKeyExportModel model);
        Task<TemporaryExposureKeyExportModel[]> GetKeysAsync(ulong sinceEpochSeconds);
        Task<TemporaryExposureKeyExportModel[]> GetKeysAsync(ulong sinceEpochSeconds, string region);
        Task<TemporaryExposureKeyExportModel[]> GetOutOfTimeKeysAsync();
    }
}
