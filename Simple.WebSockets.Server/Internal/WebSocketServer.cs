namespace Simple.WebSockets.Server.Internal
{
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Shared.Helpers;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public class WebSocketServer : IWebSocketServer
    {
        private const string CLOSE_MESSAGE = "Socket closed by the server";
        
        private readonly INotificationService NotificationService;
        private readonly IServiceScopeFactory ServiceScopeFactory;

        internal ConcurrentDictionary<string, SocketConnection> Connections { get; set; } = new ConcurrentDictionary<string, SocketConnection>();

        /// <summary>
        /// Initialises a new instance of the <see cref="WebSocketServer"/> class
        /// </summary>
        public WebSocketServer(IServiceScopeFactory serviceScopeFactory, INotificationService notificationService)
        {
            this.ServiceScopeFactory = serviceScopeFactory;
            this.NotificationService = notificationService;
        }

        /// <inheritdoc />
        public int ActiveConnections
        {
            get
            {
                var connectionsCount = this.Connections?.Count;
                if (connectionsCount != null)
                {
                    return (int) connectionsCount;
                }

                return 0;
            }
        }

        /// <inheritdoc />
        public event EventHandler<SocketConnection> OnClientConnected;

        /// <inheritdoc />
        public event EventHandler<SocketConnection> OnClientDisconnected;

        /// <inheritdoc />
        public event EventHandler<string> OnMessageReceived;

        /// <inheritdoc />
        public SocketConnection GetSocketById(string id)
        {
            return this.Connections.FirstOrDefault(p => p.Key == id).Value;
        }

        /// <inheritdoc />
        public IList<SocketConnection> GetAll()
        {
            return this.Connections.Values.ToList();
        }

        /// <inheritdoc />
        public SocketConnection GetById(string socketId)
        {
            this.Connections.TryGetValue(socketId, out var connection);
            return connection;
        }

        /// <inheritdoc />
        public async Task<string> AddSocketAsync(WebSocket socket, string clientName)
        {
            var socketId = Utilities.GenerateIdentifier();
            var socketConnection = new SocketConnection(socket, clientName);
            
            this.Connections.TryAdd(socketId, socketConnection);

            this.OnClientConnected?.Invoke(this, socketConnection);

            using (var scope = this.ServiceScopeFactory.CreateScope())
            {
                var notificationService = scope.ServiceProvider.GetService<INotificationService>();
                if (notificationService != null)
                {
                    await this.NotificationService.OnClientConnected(socketConnection);
                }
            }

            return socketId;
        }

        /// <inheritdoc />
        public async Task RemoveSocketAsync(string socketId)
        {
            this.Connections.TryRemove(socketId, out var socketConnection);
            if (socketConnection != null)
            {
                await socketConnection.CloseAsync(WebSocketCloseStatus.NormalClosure, CLOSE_MESSAGE);
            }

            this.OnClientDisconnected?.Invoke(this, socketConnection);

            using (var scope = this.ServiceScopeFactory.CreateScope())
            {
                var notificationService = scope.ServiceProvider.GetService<INotificationService>();
                if (notificationService != null)
                {
                    await this.NotificationService.OnClientDisconnected(socketConnection);
                }
            }
        }

        /// <inheritdoc />
        public async Task MessageReceivedAsync(string message)
        {
            await Task.CompletedTask;
            
            this.OnMessageReceived?.Invoke(this, message);

            using (var scope = this.ServiceScopeFactory.CreateScope())
            {
                var notificationService = scope.ServiceProvider.GetService<INotificationService>();
                if (notificationService != null)
                {
                    await this.NotificationService.OnMessageReceived(message);
                }
            }
        }
    }
}
