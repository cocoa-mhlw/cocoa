/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿namespace Covid19Radar.Api.Models
{
    public class CustomVerificationStatusModel
    {
        public string id { get; set; }
        public string Result { get; set; }
        public int HttpStatusCode { get; set; }
    }
}
