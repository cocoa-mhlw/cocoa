using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace Covid19Radar.Tests.Mock
{
    public class HttpContextMock : HttpContext
    {
        public HttpContextMock()
        {
            _Request = new HttpRequestMock(this);
            _Response = new HttpResponseMock(this);
            _Authentication = new AuthenticationManagerMock(this);
        }

        public IFeatureCollection _Feature = new FeatureCollection();
        public override IFeatureCollection Features => _Feature;

        private HttpRequest _Request;
        public override HttpRequest Request => _Request;

        private HttpResponse _Response;
        public override HttpResponse Response => _Response;

        public ConnectionInfo _Connection = new ConnectionInfoMock();
        public override ConnectionInfo Connection => _Connection;

        public WebSocketManager _WebSockets = new WebSocketManagerMock();
        public override WebSocketManager WebSockets => _WebSockets;

        public AuthenticationManager _Authentication;
        public override AuthenticationManager Authentication => _Authentication;

        public override ClaimsPrincipal User { get; set; }
        public override IDictionary<object, object> Items { get; set; }
        public override IServiceProvider RequestServices { get; set; }
        public override CancellationToken RequestAborted { get; set; }
        public override string TraceIdentifier { get; set; }
        public override ISession Session { get; set; }

        public override void Abort()
        {
        }
    }
}
