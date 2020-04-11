using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Covid19Radar.Tests.Mock
{
    public class HttpRequestMock : HttpRequest
    {

        public HttpRequestMock(HttpContext context)
        {
            _HttpContext = context;
        }

        private HttpContext _HttpContext;
        public override HttpContext HttpContext => _HttpContext;

        public override string Method { get; set; }
        public override string Scheme { get; set; }
        public override bool IsHttps { get; set; } = true;
        public override HostString Host { get; set; }
        public override PathString PathBase { get; set; }
        public override PathString Path { get; set; }
        public override QueryString QueryString { get; set; }
        public override IQueryCollection Query { get; set; }
        public override string Protocol { get; set; }

        public IHeaderDictionary _Headers = new HeaderDictionary();
        public override IHeaderDictionary Headers => _Headers;

        public override IRequestCookieCollection Cookies { get; set; }
        public override long? ContentLength { get; set; }
        public override string ContentType { get; set; }
        public override Stream Body { get; set; }

        public bool _HasFormContentType = false;
        public override bool HasFormContentType => _HasFormContentType;

        public override IFormCollection Form { get; set; }

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Form);
        }
    }
}
