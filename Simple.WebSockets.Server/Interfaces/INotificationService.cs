namespace Simple.WebSockets.Server.Interfaces
{
    using System.Threading.Tasks;
    using Internal;

    /// <summary>
    /// Notification Service
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Notification for when a Client Connects
        /// </summary>
        Task OnClientConnected(SocketConnection connection);

        /// <summary>
        /// Notification for when a Client Disconnects
        /// </summary>
        Task OnClientDisconnected(SocketConnection connection);

        /// <summary>
        /// Notification for when a Message is Received
        /// </summary>
        Task OnMessageReceived(string message);
    }
}