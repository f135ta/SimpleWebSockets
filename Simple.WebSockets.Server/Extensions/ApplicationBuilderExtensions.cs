namespace Simple.WebSockets.Server.Extensions
{
    using Internal;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Application Builder Extensions
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the WebSocket Middleware to the Pipeline
        /// </summary>
        /// <param name="app">Application Builder</param>
        /// <returns><see cref="IApplicationBuilder"/></returns>
        public static IApplicationBuilder UseWebSocketMiddleware(this IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.UseMiddleware<WebSocketMiddleware>();
            return app;
        }
    }
}