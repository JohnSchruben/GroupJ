using SafeSkate;

internal class Program
{
    private static void Main(string[] args)
    {
        //ServiceTypeProvider.ServerIp = "172.214.88.163";
        ServiceTypeProvider.ServerIp = "127.0.0.1";
        ServiceTypeProvider.UpdatePort = 9000;
        ServiceTypeProvider.QueryPort = 9001;
        var test2 = ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy;
        test2.MapMarkerInfos.CollectionChanged += MapMarkerInfos_CollectionChanged;
        foreach (var marker in test2.MapMarkerInfos)
        {
            Console.WriteLine(marker);
        }
        while (true)
        {
            Console.ReadKey();
            test2.AddMapMarkerInfo(new MapMarkerInfo(new Coordinate(40.7128, -74.0060, 10), "console client", DateTime.Now, Severity.Morphine));
        }
    }

    private static void MapMarkerInfos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Console.WriteLine(e.NewItems[0] as MapMarkerInfo);
    }
}