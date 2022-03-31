/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Model;

namespace Covid19Radar.Services
{
    public interface IDiagnosisKeyRegisterServer
    {
        public Task<HttpStatusCode> SubmitDiagnosisKeysAsync(
            bool hasSymptom,
            DateTime symptomOnsetDate,
            IList<TemporaryExposureKey> temporaryExposureKeys,
            string processNumber,
            string idempotencyKey
            );

        public Task<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request);
    }
}
