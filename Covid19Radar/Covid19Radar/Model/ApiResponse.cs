/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Net;

namespace Covid19Radar.Model
{
    public class ApiResponse<T>
    {
        public T Result { get; }
        public int StatusCode { get; }

        public ApiResponse(int statusCode = 0, T result = default)
        {
            Result = result;
            StatusCode = statusCode;
        }
    }
}
