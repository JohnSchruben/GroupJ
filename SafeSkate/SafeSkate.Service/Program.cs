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

    static async Task Main(string[] args)
    {
        // Configure the ServiceTypeProvider with server details if needed
        ServiceTypeProvider.ServerIp = "localhost";
        ServiceTypeProvider.UpdatePort = 9000;
        ServiceTypeProvider.QueryPort = 9001;

        // default map marker list
        ServiceTypeProvider.DefaultMapMarkerInfos = new List<MapMarkerInfo>()
        {
            new MapMarkerInfo(new Coordinate(), "server", DateTime.Now, Severity.High) 
        };  

        Task updateServerTask = RunUpdateServerAsync(ServiceTypeProvider.UpdatePort);
        Task queryServerTask = RunQueryServerAsync(ServiceTypeProvider.QueryPort);

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
    private static ConcurrentBag<TcpClient> updateClients = new ConcurrentBag<TcpClient>();

    static async Task HandleUpdateClientAsync(TcpClient client)
    {
        try
        {
            var stream = client.GetStream();
            var reader = new StreamReader(stream, Encoding.UTF8);
            var json = await reader.ReadToEndAsync();
            var message = JsonSerializer.Deserialize<MapMarkerUpdateMessage>(json);

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
            Console.WriteLine($"Error handling update client: {ex.Message}");
        }
        finally
        {
            if (!updateClients.Contains(client))
            {
                updateClients.Add(client);
            }
        }
    }

    // When broadcasting updates:
    public static void BroadcastUpdate(MapMarkerUpdateMessage updatedMarkerInfo)
    {
        foreach (var client in updateClients)
        {
            try
            {
                var stream = client.GetStream();
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                var json = JsonSerializer.Serialize(updatedMarkerInfo);

                writer.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error broadcasting update: {ex.Message}");
                // Handle disconnected clients
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
            var json = JsonSerializer.Serialize(markers);

            await writer.WriteAsync(json + Environment.NewLine); // Ensure newline for client-side reading
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
