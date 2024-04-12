using SafeSkate;
using System;

internal class Program
{
    private static void Main(string[] args)
    {
        Random random = new Random();
        double minLatitude = -90;
        double maxLatitude = 90;
        double minLongitude = -180;
        double maxLongitude = 180;
        ServiceTypeProvider.ServerIp = "172.214.88.163";
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
            Thread.Sleep(TimeSpan.FromSeconds(.2));
            //Console.ReadKey();
            // Generate a random latitude and longitude within the range
            double randomLatitude = random.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
            double randomLongitude = random.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

            model.AddMapMarkerInfo(
                new MapMarkerInfo(new Coordinate(randomLatitude, randomLongitude, 10), "john's mobile client", DateTime.Now, Severity.Morphine));
        }
    }

    private static void MapMarkerInfos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Console.WriteLine(e.NewItems[0] as MapMarkerInfo);
    }
}