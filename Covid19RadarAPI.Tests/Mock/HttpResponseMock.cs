using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Tests.Mock
{
    public class HttpResponseMock : HttpResponse
    {
        public HttpResponseMock(HttpContext context)
        {
            _HttpContext = context;
        }

        private HttpContext _HttpContext;

        public override HttpContext HttpContext => _HttpContext;

        public override int StatusCode { get; set; }

        public IHeaderDictionary _Headers = new HeaderDictionary();
        public override IHeaderDictionary Headers => _Headers;

        public override Stream Body { get; set; }
        public override long? ContentLength { get; set; }
        public override string ContentType { get; set; }

        public IResponseCookies _Cookies = new ResponseCookiesMock();
        public override IResponseCookies Cookies => _Cookies;

        public bool HasStartedValue;
        public override bool HasStarted => HasStartedValue;

        public override void OnCompleted(Func<object, Task> callback, object state)
        {

        }

        public override void OnStarting(Func<object, Task> callback, object state)
        {

        }

        public override void Redirect(string location, bool permanent)
        {

        }
    }
}
