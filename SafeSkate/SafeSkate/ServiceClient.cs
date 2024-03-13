using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class ServiceClient
    {
        private readonly string serverIp;
        private readonly int updatePort;
        private readonly int queryPort;
        private TcpClient updateClient;
        private TcpClient queryClient;
        private CancellationTokenSource cancellationTokenSource;

        public ServiceClient(string serverIp, int updatePort, int queryPort)
        {
            this.serverIp = serverIp;
            this.updatePort = updatePort;
            this.queryPort = queryPort;
            this.updateClient = new TcpClient();
            this.queryClient = new TcpClient(); 
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async void PublishMapMarkerUpdate(MapMarkerUpdateMessage message)
        {
            try
            {
                using var stream = queryClient.GetStream();
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                var reader = new StreamReader(stream, Encoding.UTF8);

                // Send the query to the server
                await writer.WriteLineAsync(JsonSerializer.Serialize(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while querying markers: {ex.Message}");
            }
        }

        public async Task<List<MapMarkerInfo>> QueryMarkersAsync(string query)
        {
            List<MapMarkerInfo> markers = new List<MapMarkerInfo>();
            if (!queryClient.Connected)
            {
                Console.WriteLine("Client is not connected.");
                return markers;
            }

            try
            {
                using var stream = queryClient.GetStream();
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                var reader = new StreamReader(stream, Encoding.UTF8);

                // Send the query to the server
                await writer.WriteLineAsync(query);

                // Wait for the server's response
                var response = await reader.ReadLineAsync();
                if (response != null)
                {
                    // Deserialize the response back into a list of MapMarkerInfo objects
                    markers = JsonSerializer.Deserialize<List<MapMarkerInfo>>(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while querying markers: {ex.Message}");
            }

            return markers;
        }
        public async Task StartAsync()
        {
            try
            {
                await updateClient.ConnectAsync(serverIp, updatePort);
                Console.WriteLine("Connected to the server.");
                await ListenForUpdatesAsync(cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not connect to the server: {ex.Message}");
            }
        }

        public event Action<MapMarkerUpdateMessage> MapMarkerUpdateReceived;
        private async Task ListenForUpdatesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var stream = updateClient.GetStream();
                var reader = new StreamReader(stream, Encoding.UTF8);

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (updateClient.Available > 0)
                    {
                        var message = await reader.ReadLineAsync();
                        if (message != null)
                        {
                            Console.WriteLine($"Received update: {message}");
                            var markerInfo = JsonSerializer.Deserialize<MapMarkerUpdateMessage>(message);
                            this.MapMarkerUpdateReceived?.Invoke(markerInfo);
                        }
                    }
                    else
                    {
                        await Task.Delay(100, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Listening for updates was canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while listening for updates: {ex.Message}");
            }
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            updateClient.Close();
            Console.WriteLine("Disconnected from the server.");
        }
    }
}
