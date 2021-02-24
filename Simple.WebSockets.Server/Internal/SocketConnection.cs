namespace Simple.WebSockets.Server.Internal
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Shared.Helpers;

    /// <summary>
    /// Web Socket Connection
    /// </summary>
    public class SocketConnection
    {
        private WebSocket Socket { get; }
        private readonly CancellationTokenSource CancellationTokenSource;

        /// <summary>
        /// Gets or sets the Socket Connection Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Client Name
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="SocketConnection"/> class
        /// </summary>
        /// <param name="socket">Web Socket</param>
        /// <param name="clientName">Client Name</param>
        public SocketConnection(WebSocket socket, string clientName)
        {
            this.Socket = socket;
            this.CancellationTokenSource = new CancellationTokenSource();
            this.Id = Utilities.GenerateIdentifier();
            this.ClientName = string.IsNullOrEmpty(clientName) ? $"client.{this.Id}" : clientName.ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        /// <returns><see cref="Task"/></returns>
        public async Task SendAsync(WebSocket socket, string data)
        {
            if (socket.State != WebSocketState.Open)
            {
                return;
            }

            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
                
            await socket.SendAsync(segment, WebSocketMessageType.Text, true, this.CancellationTokenSource.Token);
        }

        /// <summary>
        /// Close Socket
        /// </summary>
        /// <param name="normalClosure">Closure Reason</param>
        /// <param name="closeMessage">Close Message</param>
        /// <returns><see cref="Task"/></returns>
        public async Task CloseAsync(WebSocketCloseStatus normalClosure, string closeMessage)
        {
            await this.Socket.CloseAsync(normalClosure, closeMessage, this.CancellationTokenSource.Token);
        }
    }
}