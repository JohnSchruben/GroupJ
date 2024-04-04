using SafeSkate;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{

    private static ConcurrentBag<TcpClient> updateClients = new ConcurrentBag<TcpClient>();
    static ObservableCollection<MapMarkerInfo> markers; 
    static async Task Main2(string[] args)
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

            // Wait until data is available
            while (!stream.DataAvailable)
            {
                await Task.Delay(100); 
            }

            var reader = new StreamReader(stream, Encoding.UTF8);
            var xml = await reader.ReadToEndAsync();

            var message = SafeSkateSerializer.DeserializeMapMarkerUpdateMessage(xml);

            Console.WriteLine($"HandleUpdateClientAsync:{xml}");
            if (message.IsAdded)
            {
                markers.Add(message.Info);
            }

            if (!updateClients.Contains(client))
            {
                updateClients.Add(client);
            }

            BroadcastUpdate(message, client);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling update client: {ex.Message} {ex.StackTrace}");
        }
    }
    public async static void BroadcastUpdate(MapMarkerUpdateMessage updatedMarkerInfo, TcpClient origin)
    {
        foreach (var client in updateClients/*.Where(x => x != origin)*/)
        {
            try
            {
                Console.WriteLine($"Sending back to client");
                if (client.Connected)
                {
                    var stream = client.GetStream();
                    var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                    var xml = SafeSkateSerializer.SerializeMapMarkerUpdateMessage(updatedMarkerInfo);

                    await writer.WriteAsync(xml + Environment.NewLine);
                    //var xml = SafeSkateSerializer.SerializeMapMarkerUpdateMessage(updatedMarkerInfo);

                    //using (var stream = client.GetStream())
                    //using (var writer = new StreamWriter(stream, Encoding.UTF8))
                    //{
                    //    writer.AutoFlush = true;
                    //    writer.Write(xml);
                    //}
                }
                else
                {
                    Console.WriteLine($"client not connected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error broadcasting update: {ex.Message}");
            }
        }


        Console.WriteLine($"BroadcastUpdate done");
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
}
