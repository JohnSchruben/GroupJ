using SafeSkate;
using System;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        IProximityNotifier proximityNotifier = new ProximityNotifier();
        proximityNotifier.OnProximityReached += ProximityNotifier_OnProximityReached;

        // set the coordinates close to eachother.
        Coordinate p1 = new Coordinate();
        Coordinate p2 = new Coordinate();
        var marker = new MapMarkerInfo()
        {
            Location = p2,
        };

        var markerlist = new List<MapMarkerInfo>() { marker };

        // test should raise event.
        proximityNotifier.AssessProximity(p1, markerlist);

        // set coordinates far apart.
        p1 = new Coordinate(35.207554, -97.444606, 11);
        p2 = new Coordinate(36.208554, -98.449606, 11);
        marker = new MapMarkerInfo()
        {
            Location = p2,
        };

        markerlist = new List<MapMarkerInfo>() { marker };

        // test should raise event.
        proximityNotifier.AssessProximity(p1, markerlist);
    }

    private static void ProximityNotifier_OnProximityReached(MapMarkerInfo obj)
    {
        Console.WriteLine("Event raised");
    }
}