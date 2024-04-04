using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SafeSkate
{

    public class ServiceClient
    {
        private readonly string serverIp;
        private readonly int updatePort;
        private readonly int queryPort;
        private TcpClient updateClient;
        private TcpClient queryClient;
        private CancellationTokenSource cancellationTokenSource;
        private NetworkStream stream;
        private StreamReader reader;

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

                this.updateClient = new TcpClient();
                await updateClient.ConnectAsync(serverIp, updatePort);
                Console.WriteLine("Connected to the server.");
                this.stream = updateClient.GetStream();
                this.reader = new StreamReader(stream, Encoding.UTF8);

                //using var stream = updateClient.GetStream();
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                
                // Send the query to the server
                await writer.WriteLineAsync(SafeSkateSerializer.SerializeMapMarkerUpdateMessage(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while publishing an update: {ex.Message}");
            }
            finally
            {
                this.updateClient.Dispose();
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
            try
            {
                await updateClient.ConnectAsync(serverIp, updatePort);
                Console.WriteLine("Connected to the server.");
                this.stream = updateClient.GetStream();
                this.reader = new StreamReader(stream, Encoding.UTF8);
                ListenForUpdatesAsync(cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public event Action<MapMarkerUpdateMessage> MapMarkerUpdateReceived;
        private async Task ListenForUpdatesAsync(CancellationToken cancellationToken)
        {
            try
            {

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!updateClient.Connected)
                    {
                        this.updateClient = new TcpClient();
                        await updateClient.ConnectAsync(serverIp, updatePort);
                        Console.WriteLine("Connected to the server.");
                        this.stream = updateClient.GetStream();
                        this.reader = new StreamReader(stream, Encoding.UTF8);
                        continue;
                    }

                    if ( updateClient.Available > 0)
                    {
                        var message = await reader.ReadLineAsync();
                        if (message != null)
                        {
                            Console.WriteLine($"Received update: {message}");
                            var markerInfo = SafeSkateSerializer.DeserializeMapMarkerUpdateMessage(message);
                            this.MapMarkerUpdateReceived?.Invoke(markerInfo);
                        }
                    }
                    else
                    {
                        await Task.Delay(100, cancellationToken);
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
            cancellationTokenSource.Cancel();
            updateClient.Close();
            Console.WriteLine("Disconnected from the server.");
        }
    }
}
