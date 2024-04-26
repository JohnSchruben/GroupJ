using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate
{
    public interface IProximityNotifier
    {
        event Action<MapMarkerInfo> OnProximityReached; 

        void AssessProximity(Coordinate currentUserLocation, IEnumerable<MapMarkerInfo> mapMarkers);
    }
}
