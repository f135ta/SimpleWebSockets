namespace Simple.WebSockets.Client.Interfaces
{
    using System;
    using System.Net.WebSockets;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface: WebSocket Client
    /// </summary>
    public interface IWebSocketClient
    {
        /// <summary>
        /// Event: OnMessageReceived
        /// </summary>
        event EventHandler<string> OnMessageReceived;

        /// <summary>
        /// Event: OnConnected
        /// </summary>
        event EventHandler OnConnected;

        /// <summary>
        /// Event: OnDisconnected
        /// </summary>
        event EventHandler OnDisconnected;

        /// <summary>
        /// Gets the WebSocket Connection State
        /// </summary>
        WebSocketState ConnectionState { get; }

        /// <summary>
        /// Connects the WebSocket Server
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task ConnectAsync();

        /// <summary>
        /// Disconnects from the WebSocket Server
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task DisconnectAsync();

        /// <summary>
        /// Send Message to the Server
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task SendAsync(string message);

        /// <summary>
        /// Dispose
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        void Dispose();
    }
}