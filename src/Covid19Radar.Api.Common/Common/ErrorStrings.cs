/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Api.Common
{
    public class ErrorStrings
    {

        //1000: User error strings
        public const string UserNotFound = "1001: User not found.";

        //2000: OTP error strings
        public const string OtpInvalidValidateRequest = "2001: Invalid Otp validate request.";
        public const string OtpSmsSendFailure = "2001: Failed to send SMS.";

        //9000: System error strings
        public const string PayloadInvalid = "9001: Invalid payload.";
    }
}
