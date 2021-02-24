namespace Simple.WebSockets.Client.Extensions
{
    using System;
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Service Collection Extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Websocket Client
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="config">Client Configuration</param>
        public static void AddWebSocketClient(this IServiceCollection services, Action<ClientConfiguration> config)
        {
            services.Configure(config);
            services.AddSingleton<IWebSocketClient, WebSocketClient>();
        }
    }
}
