using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate.Service
{
    internal class NewProgram
    {
        private static ObservableCollection<MapMarkerInfo> markers;

        static Server server = new Server();
        public static void Main(string[] args)
        {
            // Defaults
            string defaultIp = "127.0.0.1";
            int defaultUpdatePort = 9000;
            int defaultQueryPort = 9001;

            // Configure the ServiceTypeProvider with server details based on input args or use defaults
            ServiceTypeProvider.ServerIp = args.Length > 0 ? args[0] : defaultIp;
            ServiceTypeProvider.UpdatePort = args.Length > 1 && int.TryParse(args[1], out int updatePort) ? updatePort : defaultUpdatePort;
            ServiceTypeProvider.QueryPort = args.Length > 2 && int.TryParse(args[2], out int queryPort) ? queryPort : defaultQueryPort;

            // Default map marker list. Will turn into a file read or a database.
            //ServiceTypeProvider.DefaultMapMarkerInfos = new List<MapMarkerInfo>
            markers = new ObservableCollection<MapMarkerInfo>
            {
                 new MapMarkerInfo(new Coordinate(35.211166, -97.441197, 10), "server", DateTime.Now, Severity.Morphine)
            };

            Console.WriteLine($"starting service host on {ServiceTypeProvider.ServerIp}");

            server.StartServer(ServiceTypeProvider.UpdatePort, markers);

            Task queryServerTask = RunQueryServerAsync(ServiceTypeProvider.QueryPort);

            Task.WhenAll(queryServerTask);
            Console.ReadKey();
        }

        static async Task HandleQueryClientAsync(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // send all markers upon client connection
                //var markers = ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.MapMarkerInfos;
                var xml = SafeSkateSerializer.SerializeMapMarkerInfoList(markers.ToList());

                await writer.WriteAsync(xml + Environment.NewLine);
                Console.WriteLine("Markers sent to query client.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling query client: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        static async Task RunQueryServerAsync(int port)
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Query server listening on port {port}...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = HandleQueryClientAsync(client);
            }
        }
    }
}
