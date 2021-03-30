/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿namespace Covid19Radar.Api.Models
{
    public class SequenceModel
    {
        public string id { get; set; }
        public string PartitionKey { get; set; }
        public ulong value { get; set; }
        public string _self { get; set; }
    }
}
