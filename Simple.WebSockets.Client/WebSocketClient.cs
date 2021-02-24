namespace Simple.WebSockets.Client
{
    using System;
    using System.IO;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Interfaces;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Web Socket Client
    /// </summary>
    public class WebSocketClient : IWebSocketClient
    {
        private ClientWebSocket Client;
        private CancellationTokenSource CancellationTokenSource;

        private const string CLIENT_NAME = "x-name";

        private readonly ClientConfiguration Configuration;

        /// <inheritdoc />
        public WebSocketState ConnectionState => this.Client.State;

        /// <inheritdoc />
        public event EventHandler<string> OnMessageReceived;

        /// <inheritdoc />
        public event EventHandler OnConnected;

        /// <inheritdoc />
        public event EventHandler OnDisconnected;

        /// <summary>
        /// Initialises a new instance of the <see cref="WebSocketClient"/> class
        /// </summary>
        /// <param name="configuration">Client Configuration</param>
        public WebSocketClient(IOptions<ClientConfiguration> configuration)
        {
            this.Configuration = configuration.Value;
            this.CancellationTokenSource = new CancellationTokenSource();
        }

        /// <inheritdoc />
        public async Task ConnectAsync()
        {
            if (this.Client != null)
            {
                if (this.Client.State == WebSocketState.Open)
                {
                    return;
                }

                this.Client.Dispose();
            }

            var cancellationToken = new CancellationTokenSource(); //todo: Get this from the ApplicationLifetime Context

            this.Client = new ClientWebSocket();
            this.Client.Options.SetRequestHeader(CLIENT_NAME, this.Configuration.ClientName);

            // Connect to the Server
            await this.Client.ConnectAsync(new Uri(this.Configuration.ConnectionUrl), cancellationToken.Token);

            if (this.Client.State == WebSocketState.Open)
            {
                this.OnConnected?.Invoke(this, null);
            }

            // Wait for incoming messages
            await Task.Factory.StartNew(() => this.InternalReceiveLoop(cancellationToken.Token), cancellationToken.Token);
        }

        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            if (this.Client is null)
            {
                return;
            }

            if (this.Client.State == WebSocketState.Open)
            {
                this.CancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(2));

                await this.Client.CloseOutputAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
                await this.Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }

            this.Client.Dispose();
            this.Client = null;

            this.CancellationTokenSource.Dispose();
            this.CancellationTokenSource = null;

            this.OnDisconnected?.Invoke(this, null);
        }

        /// <inheritdoc />
        public async Task SendAsync(string message)
        {
            if (this.ConnectionState != WebSocketState.Open)
            {
                await this.ConnectAsync();
            }

            var segment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));

            await this.Client.SendAsync(segment, WebSocketMessageType.Text, true, this.CancellationTokenSource.Token);
        }

        private async Task InternalReceiveLoop(CancellationToken token)
        {
            MemoryStream outputStream = null;

            var buffer = new byte[this.Configuration.ReceiveBufferSize];
            try
            {
                while (!token.IsCancellationRequested)
                {
                    outputStream = new MemoryStream(this.Configuration.ReceiveBufferSize);
                    WebSocketReceiveResult receiveResult;

                    do
                    {
                        receiveResult = await this.Client.ReceiveAsync(buffer, this.CancellationTokenSource.Token);
                        if (receiveResult.MessageType != WebSocketMessageType.Close)
                        {
                            outputStream.Write(buffer, 0, receiveResult.Count);
                        }

                    } while (!receiveResult.EndOfMessage);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }

                    outputStream.Position = 0;

                    this.OnMessageReceived?.Invoke(this, Encoding.UTF8.GetString(outputStream.ToArray()));
                }
            }
            catch (TaskCanceledException)
            {

            }
            finally
            {
                outputStream?.Dispose();
            }
        }

        /// <inheritdoc />
        public void Dispose() => this.DisconnectAsync().Wait();
    }
}
