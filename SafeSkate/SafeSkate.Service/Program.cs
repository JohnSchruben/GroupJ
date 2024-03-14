using SafeSkate;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{

    private static ConcurrentBag<TcpClient> updateClients = new ConcurrentBag<TcpClient>();
    static async Task Main(string[] args)
    {
        // Defaults
        string defaultIp = "localhost";
        int defaultUpdatePort = 9000;
        int defaultQueryPort = 9001;

        // Configure the ServiceTypeProvider with server details based on input args or use defaults
        ServiceTypeProvider.ServerIp = args.Length > 0 ? args[0] : defaultIp;
        ServiceTypeProvider.UpdatePort = args.Length > 1 && int.TryParse(args[1], out int updatePort) ? updatePort : defaultUpdatePort;
        ServiceTypeProvider.QueryPort = args.Length > 2 && int.TryParse(args[2], out int queryPort) ? queryPort : defaultQueryPort;

        // Default map marker list. Will turn into a file read or a database.
        ServiceTypeProvider.DefaultMapMarkerInfos = new List<MapMarkerInfo>
        {
             new MapMarkerInfo(new Coordinate(40.7128, -74.0060, 10), "server", DateTime.Now, Severity.Morphine)
        };

        Console.WriteLine($"starting service host on {ServiceTypeProvider.ServerIp}");

        // Start server tasks
        Task updateServerTask = RunUpdateServerAsync(ServiceTypeProvider.UpdatePort);
        Task queryServerTask = RunQueryServerAsync(ServiceTypeProvider.QueryPort);

        // Await tasks
        await Task.WhenAll(updateServerTask, queryServerTask);
    }

    static async Task RunUpdateServerAsync(int port)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Update server listening on port {port}...");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = HandleUpdateClientAsync(client);
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

    static async Task HandleUpdateClientAsync(TcpClient client)
    {
        try
        {
            var stream = client.GetStream();
            var reader = new StreamReader(stream, Encoding.UTF8);
            var xml = await reader.ReadToEndAsync();
            var message = SafeSkateSerializer.DeserializeMapMarkerUpdateMessage(xml);

            Console.WriteLine($"HandleUpdateClientAsync:{xml}");
            if (message.IsAdded)
            {
                ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.AddMapMarkerInfo(message.Info);
            }
            else
            {
                ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.RemoveMapMarkerInfo(message.Info);
            }

            BroadcastUpdate(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling update client: {ex.Message} {ex.StackTrace}");
        }
        finally
        {
            if (!updateClients.Contains(client))
            {
                updateClients.Add(client);
            }
        }
    }

    public static void BroadcastUpdate(MapMarkerUpdateMessage updatedMarkerInfo)
    {
        foreach (var client in updateClients)
        {
            try
            {
                var stream = client.GetStream();
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                var xml = SafeSkateSerializer.SerializeMapMarkerUpdateMessage(updatedMarkerInfo);

                writer.WriteLine(xml);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error broadcasting update: {ex.Message}");
            }
        }
    }

    static async Task HandleQueryClientAsync(TcpClient client)
    {
        try
        {
            var stream = client.GetStream();
            var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            // Automatically send all markers upon client connection
            var markers = ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.MapMarkerInfos;

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
}
