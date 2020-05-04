using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Tests.Mock
{
    public class WebSocketManagerMock : WebSocketManager
    {
        public bool IsWebSocketRequestValue = false;
        public override bool IsWebSocketRequest => IsWebSocketRequestValue;

        public List<string> WebSocketRequestedProtocolsValue = new List<string>();
        public override IList<string> WebSocketRequestedProtocols => WebSocketRequestedProtocolsValue;

        public WebSocketMock WebSocket = new WebSocketMock();
        public override Task<WebSocket> AcceptWebSocketAsync(string subProtocol)
        {
            return Task.FromResult((WebSocket)WebSocket);
        }

        public class WebSocketMock : WebSocket
        {
            public WebSocketCloseStatus? CloseStatusValue = null;
            public override WebSocketCloseStatus? CloseStatus => CloseStatusValue;

            public string CloseStatusDescriptionValue;
            public override string CloseStatusDescription => CloseStatusDescriptionValue;

            public WebSocketState StateValue;
            public override WebSocketState State => StateValue;

            public string SubProtocolValue;
            public override string SubProtocol => SubProtocolValue;

            public override void Abort()
            {
            }

            public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public override void Dispose()
            {
            }

            public WebSocketReceiveResult ReceiveAsyncValue;
            public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
            {
                return Task.FromResult(ReceiveAsyncValue);
            }

            public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
