/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Net.Http;

namespace Covid19Radar.Services
{
    public interface IHttpClientService
    {
        HttpClient Create();
    }
    public class HttpClientService : IHttpClientService
    {
        public HttpClientService()
        {
        }
        public HttpClient Create()
        {
            return new HttpClient();
        }
    }
}
