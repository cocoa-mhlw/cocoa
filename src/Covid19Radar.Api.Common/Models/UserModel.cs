﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Api.Models
{
    public class UserModel : IUser
    {
        /// <summary>
        /// for CosmosDB id
        /// </summary>
        public string id { get => this.GetId(); }

        /// <summary>
        /// for CosmosDB PartitionKey
        /// </summary>
        public string PartitionKey { get => this.GetId(); }

        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        public string UserUuid { get; set; }

        /// <summary>
        /// Protected Secret key
        /// </summary>
        /// <value>Protected Secret Key</value>
        public string ProtectSecret { get; set; }

        /// <summary>
        /// Jump Consistent Seed 
        /// </summary>
        /// <value>Jump Consistent Hash</value>
        public ulong JumpConsistentSeed { get; set; }

        /// <summary>
        /// ETAG
        /// </summary>
        public string _etag { get; set; }
    }
}
