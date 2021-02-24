namespace TestServer
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Simple.WebSockets.Server.Interfaces;
    using Simple.WebSockets.Server.Internal;

    /// <summary>
    /// Processes events from the WebSocket Server
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ILogger Logger;

        public NotificationService(ILoggerFactory loggerFactory)
        {
            this.Logger = loggerFactory.CreateLogger<NotificationService>();
        }

        public async Task OnClientConnected(SocketConnection connection)
        {
            this.Logger.LogDebug($"{connection.ClientName} connected");
            await Task.CompletedTask;
        }

        public async Task OnClientDisconnected(SocketConnection connection)
        {
            this.Logger.LogDebug($"{connection.ClientName} disconnected");
            await Task.CompletedTask;
        }

        public async Task OnMessageReceived(string message)
        {
            this.Logger.LogDebug($"[RCVD]: {message}");
            await Task.CompletedTask;
        }
    }
}
