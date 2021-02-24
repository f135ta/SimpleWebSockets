namespace Simple.WebSockets.Server.Extensions
{
    using Interfaces;
    using Internal;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Service Collection Extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the WebSocket Manager to Dependency Injection
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <returns></returns>
        public static void AddWebSocketManager<TNotificationService>(this IServiceCollection services) where TNotificationService : class, INotificationService
        {
            services.AddSingleton<INotificationService, TNotificationService>();
            services.AddSingleton<IWebSocketServer, WebSocketServer>();
        }
    }
}