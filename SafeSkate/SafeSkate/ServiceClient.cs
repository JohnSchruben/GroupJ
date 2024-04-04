using System.Net.Sockets;
using System.Text;

namespace SafeSkate
{
    public class ServiceClient
    {
        private readonly string serverIp;
        private readonly int updatePort;
        private readonly int queryPort;

        private TcpClient updateClient;
        private TcpClient queryClient;

        private NetworkStream stream;
        private StreamReader reader;

        public ServiceClient(string serverIp, int updatePort, int queryPort)
        {
            this.serverIp = serverIp;
            this.updatePort = updatePort;
            this.queryPort = queryPort;
            this.updateClient = new TcpClient();
            this.queryClient = new TcpClient();
        }

        public async void PublishMapMarkerUpdate(MapMarkerUpdateMessage message)
        {
            try
            {
                if (!this.updateClient.Connected)
                {
                    await updateClient.ConnectAsync(serverIp, updatePort);
                }

                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // Send the query to the server
                string xml = SafeSkateSerializer.SerializeMapMarkerUpdateMessage(message);
                await writer.WriteAsync(xml);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while publishing an update: {ex.Message}");
            }
        }

        public async Task<List<MapMarkerInfo>> QueryMarkersAsync(string query)
        {
            // Ensure to connect to the correct port, which is queryPort for queries.
            await queryClient.ConnectAsync(serverIp, queryPort);
            List<MapMarkerInfo> markers = new List<MapMarkerInfo>();

            if (!queryClient.Connected)
            {
                throw new Exception();
            }

            try
            {
                using var stream = queryClient.GetStream();
                var reader = new StreamReader(stream, Encoding.UTF8);

                // Wait for the server's response.
                var response = await reader.ReadToEndAsync();
                if (response != null)
                {
                    // Deserialize the response back into a list of MapMarkerInfo objects.
                    markers = SafeSkateSerializer.DeserializeMapMarkerInfoList(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while querying markers: {ex.Message}");
            }
            finally
            {
                queryClient.Close();
            }

            return markers;
        }

        public async Task StartAsync()
        {
            this.ListenForUpdatesAsync();
        }

        public event Action<MapMarkerUpdateMessage> MapMarkerUpdateReceived;

        private async Task ListenForUpdatesAsync()
        {
            try
            {
                while (true)
                {
                    if (!updateClient.Connected)
                    {
                        this.updateClient = new TcpClient();
                        await updateClient.ConnectAsync(serverIp, updatePort);
                        Console.WriteLine("Connected to the server.");
                        this.stream = updateClient.GetStream();
                    }

                    if (stream.DataAvailable)
                    {
                        this.reader = new StreamReader(stream, Encoding.UTF8);
                        int bufferSize = 4096;
                        char[] buffer = new char[bufferSize];

                        int bytesRead;
                        while ((bytesRead = await reader.ReadAsync(buffer, 0, bufferSize)) > 0)
                        {
                            string receivedData = new string(buffer, 0, bytesRead);
                            Console.WriteLine($"Received data: {receivedData}");
                            var markerInfo = SafeSkateSerializer.DeserializeMapMarkerUpdateMessage(receivedData);
                            if (markerInfo != null)
                            {
                                 this.MapMarkerUpdateReceived?.Invoke(markerInfo);
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Client Disconnected {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Stop()
        {
            updateClient.Close();
            Console.WriteLine("Disconnected from the server.");
        }
    }
}