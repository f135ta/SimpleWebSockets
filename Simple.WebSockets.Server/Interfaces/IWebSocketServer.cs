namespace Simple.WebSockets.Server.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Threading.Tasks;
    using Internal;

    /// <summary>
    /// WebSocket Server
    /// </summary>
    public interface IWebSocketServer
    {
        /// <summary>
        /// Event: Client Connected
        /// </summary>
        event EventHandler<SocketConnection> OnClientConnected;

        /// <summary>
        /// Event: Client Disconnected
        /// </summary>
        event EventHandler<SocketConnection> OnClientDisconnected;

        /// <summary>
        /// Event: Message Received
        /// </summary>
        event EventHandler<string> OnMessageReceived;

        /// <summary>
        /// Gets a count of active connections
        /// </summary>
        int ActiveConnections { get; }

        /// <summary>
        /// Gets a Socket by Id
        /// </summary>
        /// <param name="id">Socket Id</param>
        /// <returns><see cref="SocketConnection"/></returns>
        SocketConnection GetSocketById(string id);

        /// <summary>
        /// Returns a collection of all the connected sockets
        /// </summary>
        /// <returns><see cref="IList{T}"/></returns>
        IList<SocketConnection> GetAll();

        /// <summary>
        /// Gets a socket connection from the internal collection
        /// </summary>
        /// <param name="socketId">Socket Identifier</param>
        /// <returns><see cref="SocketConnection"/></returns>
        SocketConnection GetById(string socketId);

        /// <summary>
        /// Adds a Socket to the internal collection
        /// </summary>
        /// <param name="socket">WebSocket</param>
        /// <param name="clientName">Client Name</param>
        Task<string> AddSocketAsync(WebSocket socket, string clientName);

        /// <summary>
        /// Removes a Socket from the internal collection
        /// </summary>
        /// <param name="id">Socket Id</param>
        /// <returns></returns>
        Task RemoveSocketAsync(string id);

        /// <summary>
        /// Message Received
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns><see cref="Task"/></returns>
        Task MessageReceivedAsync(string message);
    }
}