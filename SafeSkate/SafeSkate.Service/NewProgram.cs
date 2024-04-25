using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SafeSkate.Service
{
    /// <summary>
    ///   Ticket #4 The Server
    /// </summary>
    internal class NewProgram
    {
        private static ObservableCollection<MapMarkerInfo> markers;

        static Server server = new Server();
        public static void Main(string[] args)
        {
            // Default endpoints for queries and updates.
            string defaultIp = "172.214.88.163";
            int defaultUpdatePort = 9000;
            int defaultQueryPort = 9001;

            // Configure the ServiceTypeProvider with server details based on input args or use defaults
            ServiceTypeProvider.ServerIp = args.Length > 0 ? args[0] : defaultIp;
            ServiceTypeProvider.UpdatePort = args.Length > 1 && int.TryParse(args[1], out int updatePort) ? updatePort : defaultUpdatePort;
            ServiceTypeProvider.QueryPort = args.Length > 2 && int.TryParse(args[2], out int queryPort) ? queryPort : defaultQueryPort;

            // Default map marker list. Will turn into a file read or a database.
            markers = new ObservableCollection<MapMarkerInfo>
            {
                 new MapMarkerInfo(new Coordinate(35.211166, -97.441197, 10), "server", DateTime.Now, Severity.Morphine)
            };

            Console.WriteLine($"starting service host on {ServiceTypeProvider.ServerIp}");

            server.StartServer(ServiceTypeProvider.UpdatePort, markers);

            // start listening for requests.
            Task queryServerTask = RunQueryServerAsync(ServiceTypeProvider.QueryPort);
            
            Task.WhenAll(queryServerTask);
            Console.ReadKey();
        }

        // this gets called when a client connects to the query endpoint. 
        // It sends the map marker list back to the client. 
        static async Task HandleQueryClientAsync(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // send all markers upon client connection
                var xml = SafeSkateSerializer.SerializeMapMarkerInfoList(markers.ToList());

                await writer.WriteAsync(xml + Environment.NewLine);
                Console.WriteLine($"{markers.Count()} markers sent to query client.");
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

        // listinging for connections from clients. 
        static async Task RunQueryServerAsync(int port)
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Query server listening on port {port}...");

            // will listen forever as long as the task is running.
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = HandleQueryClientAsync(client);
            }
        }
    }
}
