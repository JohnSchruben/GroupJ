using SafeSkate;

internal class Program
{
    private static void Main(string[] args)
    {
        //ServiceTypeProvider.ServerIp = "172.214.88.163";
        ServiceTypeProvider.ServerIp = "localhost";
        ServiceTypeProvider.UpdatePort = 9000;
        ServiceTypeProvider.QueryPort = 9001;
        var test2 = ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy;
        foreach (var marker in test2.MapMarkerInfos)
        {
            Console.WriteLine(marker);
        }

        new MapMarkerInfo(new Coordinate(40.7128, -74.0060, 10), "console client", DateTime.Now, Severity.Morphine);

        foreach (var marker in test2.MapMarkerInfos)
        {
            Console.WriteLine(marker);
        }

        Console.ReadKey();
        return;
    }
}