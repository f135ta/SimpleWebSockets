namespace Simple.WebSockets.Server.Internal
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Web Socket Middleware
    /// </summary>
    internal class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private IWebSocketServer _webSocketServer;

        private const string CLIENT_NAME = "x-name";
        private const string CLOSURE_MESSAGE = "Bye!";

        /// <summary>
        /// Initialises a new instance of the <see cref="WebSocketMiddleware"/> class
        /// </summary>
        /// <param name="next">Request Delegate</param>
        public WebSocketMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        /// <summary>
        /// Main Invoke Method
        /// </summary>
        /// <param name="context">Http Context</param>
        /// <param name="webSocketServer">WebSocket Server</param>
        /// <returns><see cref="Task"/></returns>
        public async Task Invoke(HttpContext context, IWebSocketServer webSocketServer)
        {
            // Bypass None WebSocket Requests
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await this._next.Invoke(context);
                return;
            }

            this._webSocketServer = webSocketServer;

            // Grab the Cancellation Token for [RequestAborted]
            // todo: Fix this so that the CancellationToken is derived from the ApplicationLifetime instead!
            var cancellationToken = context.RequestAborted;

            // Accept the Incoming WebSocket
            var socket = await context.WebSockets.AcceptWebSocketAsync();

            // Get the Client Name
            var clientName = context.Request.Headers.FirstOrDefault(p => p.Key == CLIENT_NAME).Value;

            // Add the Socket to the Connection Manager
            var socketId = await this._webSocketServer.AddSocketAsync(socket, clientName);

            // Process incoming messages
            while (socket.State == WebSocketState.Open)
            {
                try
                {
                    await this.ReceiveAsync(socket, cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            // Close the Connection
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, CLOSURE_MESSAGE, cancellationToken);

            // Remove the Connection from the Connection Manager
            await this._webSocketServer.RemoveSocketAsync(socketId);
        }

        private async Task ReceiveAsync(WebSocket socket, CancellationToken cancellationToken)
        {
            // Message can be sent by chunk - We must read all chunks before decoding the content
            var buffer = new ArraySegment<byte>(new byte[8192]);
            await using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    // Exit the method if Cancellation has been requested
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    result = await socket.ReceiveAsync(buffer, cancellationToken);
                    ms.Write(buffer.Array ?? throw new InvalidOperationException(), buffer.Offset, result.Count);

                } while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                // Only process [Text] messages
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    return;
                }

                // Read the incoming data into a [string] and pass it to the WebSocketServer Management Class
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    var messageData = await reader.ReadToEndAsync();
                    await this._webSocketServer.MessageReceivedAsync(messageData);
                }
            }
        }
    }
}