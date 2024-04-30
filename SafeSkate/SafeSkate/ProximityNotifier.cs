using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate
{
    public class ProximityNotifier : IProximityNotifier
    {
        public event Action<MapMarkerInfo> OnProximityReached;

        public void AssessProximity(Coordinate currentUserLocation, IEnumerable<MapMarkerInfo> mapMarkers)
        {
            List<MapMarkerInfo> dynamicArrayforMarkers = new List<MapMarkerInfo>();
            List<double> dynamicArrayforDist = new List<double>();
            bool isCloseToMarker = false;
            
 
            foreach (var marker in mapMarkers)
            {
                //distance in meters
                double distance = CalculateHaversineDistance(currentUserLocation, marker.Location);

                double shortestDistance;
                if (distance < 10)
                {
                    dynamicArrayforMarkers.Add(marker);
                    dynamicArrayforDist.Add(distance);
                    isCloseToMarker = true;
                }
            }

            if (isCloseToMarker)
            {
                //check for closest distance. Use the position of closest point in the array
                //for the position of the Marker array
                double shortestDist = 10; // to start the logic
                int closestPointIndex = 0;
                
                for(int i = 0; i < dynamicArrayforDist.Count; i++)
                {
                    double currDist = dynamicArrayforDist[i];
                    if (currDist < shortestDist)
                    {
                        shortestDist = currDist;
                        closestPointIndex = i;
                    }
                }           

                MapMarkerInfo closestPoint = dynamicArrayforMarkers[closestPointIndex];
                OnProximityReached?.Invoke(closestPoint); 
            }

        }
        
        private double CalculateHaversineDistance(Coordinate cord1, Coordinate cord2)
        {
            //Convert from degrees to radians
            double lat1 = cord1.Latitude * Math.PI/180;
            double long1 = cord1.Longitude * Math.PI/180;;
            double lat2 = cord2.Latitude * Math.PI/180;;
            double long2 = cord2.Longitude * Math.PI/180;;
            
            
            const double r = 6378100; // radius of Earth in meters
            var dlat = Math.Sin((lat2 - lat1) / 2);
            var dlong = Math.Sin((long2 - long1) / 2);

            double ans = Math.Pow(dlat, 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(dlong, 2);

            ans = 2 * Math.Asin(Math.Sqrt(ans));
            ans = ans * r;

            return ans;
        } 
    }
}