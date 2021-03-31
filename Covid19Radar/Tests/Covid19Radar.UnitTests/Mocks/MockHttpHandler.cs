/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.UnitTests.Mocks
{
    public class MockHttpHandler : DelegatingHandler
    {
        public static HttpClient AlwaysOk() => new HttpClient(new MockHttpHandler((r, c) => new HttpResponseMessage(HttpStatusCode.OK)));

        private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler;

        public MockHttpHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler)
        {
            this.handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(handler.Invoke(request, cancellationToken));
        }
    }
}
