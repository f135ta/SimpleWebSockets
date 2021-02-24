namespace TestClient
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Simple.WebSockets.Client.Extensions;
    using Simple.WebSockets.Client.Interfaces;

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddWebSocketClient(p => p
                .WithServerUrl("wss://localhost:5001")
                .WithClientName("test-client"));

            var client = services.BuildServiceProvider().GetRequiredService<IWebSocketClient>();
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;

            // Connect the client
            await client.ConnectAsync();

            Console.WriteLine("Press Enter to stop the client or '!' to reconnect the client...");

            // Perform text input
            for (; ; )
            {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                switch (line)
                {
                    // Disconnect the client
                    case "!":
                        await client.DisconnectAsync();
                        Console.WriteLine("Done!");
                        continue;

                    // Send a special test
                    case "test10":
                        {
                            for (var i = 0; i < 10; i++)
                            {
                                await client.SendAsync($"{Guid.NewGuid()} - {i + 1}");
                            }

                            continue;
                        }

                    // Send a special test
                    case "test100":
                    {
                        for (var i = 0; i < 100; i++)
                        {
                            await client.SendAsync($"{Guid.NewGuid()} - {i + 1}");
                        }

                        continue;
                    }

                    // Send the entered text to the chat server
                    default:

                        await client.SendAsync(line);
                        break;
                }
            }
        }

        private static void Client_OnDisconnected(object sender, EventArgs e) => Console.WriteLine("Disconnected");
        private static void Client_OnConnected(object sender, EventArgs e) => Console.WriteLine("Connected");
        private static void Client_OnMessageReceived(object sender, string e) => Console.WriteLine($"[RCV] => {e}");
    }
}