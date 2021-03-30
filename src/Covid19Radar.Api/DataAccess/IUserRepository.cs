/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using Covid19Radar.Api.Models;
using System.Threading.Tasks;

#nullable enable

namespace Covid19Radar.Api.DataAccess
{
    public interface IUserRepository
    {
        Task<UserModel?> GetById(string id);

        Task Create(UserModel user);

        Task<bool> Exists(string id);

        Task<bool> Delete(IUser user);
    }
}
