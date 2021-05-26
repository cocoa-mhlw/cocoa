/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covid19Radar.Api.Services
{
    public interface IValidationInquiryLogService
    {
        ValidateResult Validate(HttpRequest req);
        public class ValidateResult
        {
            public static readonly ValidateResult Success = new ValidateResult()
            {
                IsValid = true,
                ErrorActionResult = null
            };
            public static readonly ValidateResult Error = new ValidateResult()
            {
                IsValid = false,
                ErrorActionResult = new BadRequestResult()
            };
            public bool IsValid;
            public IActionResult ErrorActionResult;
        }
    }
}
