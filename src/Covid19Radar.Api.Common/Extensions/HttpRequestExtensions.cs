﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Common;
using Covid19Radar.Api.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<T> ParseAndThrowAsync<T>(this HttpRequest request)
        where T : IPayload
        {
            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var serializedObject = JsonConvert.DeserializeObject<T>(requestBody);
            if (serializedObject == null || !serializedObject.IsValid())
            {
                throw new ArgumentException(ErrorStrings.PayloadInvalid);
            }
            return serializedObject;
        }
    }
}
