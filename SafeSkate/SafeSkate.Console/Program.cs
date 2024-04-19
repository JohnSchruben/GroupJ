using SafeSkate;
using System;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        Random random = new Random();
        double minLatitude = 35.203977;
        double maxLatitude = 35.211130;
        double minLongitude = -97.441307;
        double maxLongitude = -97.447905;
        int processId = Process.GetCurrentProcess().Id;
        int hashedValue = (processId % 4) + 1;
        Severity severity = (Severity)hashedValue;
        ServiceTypeProvider.ServerIp = "172.214.88.163";
        //ServiceTypeProvider.ServerIp = "127.0.0.1";
        ServiceTypeProvider.UpdatePort = 9000;
        ServiceTypeProvider.QueryPort = 9001;
        var model = ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy;
        model.MapMarkerInfos.CollectionChanged += MapMarkerInfos_CollectionChanged;
        var list = new List<MapMarkerInfo>(model.MapMarkerInfos);
        foreach (var marker in list)
        {
            Console.WriteLine(marker);
        }
        while (true)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            //Console.ReadKey();
            // Generate a random latitude and longitude within the range
            double randomLatitude = random.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
            double randomLongitude = random.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

            model.AddMapMarkerInfo(
                new MapMarkerInfo(new Coordinate(randomLatitude, randomLongitude, 10), "john's mobile client", DateTime.Now, severity));
        }
    }

    private static void MapMarkerInfos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Console.WriteLine(e.NewItems[0] as MapMarkerInfo);
    }
}