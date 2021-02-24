namespace Simple.WebSockets.Client.Extensions
{
    /// <summary>
    /// Client Configuration
    /// </summary>
    public class ClientConfiguration
    {
        internal string ClientName { get; set; }
        internal string ConnectionUrl { get; set; }
        internal int ReceiveBufferSize { get; set; } = 8192;

        /// <summary>
        /// Sets the Client Name
        /// </summary>
        public ClientConfiguration WithClientName(string value)
        {
            this.ClientName = value;
            return this;
        }

        /// <summary>
        /// Sets the Server Connection URL
        /// </summary>
        public ClientConfiguration WithServerUrl(string value)
        {
            this.ConnectionUrl = value;
            return this;
        }

        /// <summary>
        /// Sets the Receive Buffer Size
        /// </summary>
        public ClientConfiguration WithReceiveBufferSize(int value)
        {
            this.ReceiveBufferSize = value;
            return this;
        }
    }
}
