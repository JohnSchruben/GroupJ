using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate
{
    public class MapMarkerInfoCollection
    {
        private readonly ObservableCollection<MapMarkerInfo> mapMarkerInfos;

        public MapMarkerInfoCollection(IEnumerable<MapMarkerInfo> mapMarkerInfos)
        {
            this.mapMarkerInfos = new ObservableCollection<MapMarkerInfo>(mapMarkerInfos);
        }

        public ObservableCollection<MapMarkerInfo> MapMarkerInfos => this.mapMarkerInfos;

        public bool AddMapMarkerInfo(MapMarkerInfo mapMarkerInfo)
        {
            // don't add if it's a duplicate or if its null
            if (mapMarkerInfo != null && !mapMarkerInfos.Any(x => x.Equals(mapMarkerInfo)))
            {
                mapMarkerInfos.Add(mapMarkerInfo);
                return true;
            }

            return false;
        }

        public bool RemoveMapMarkerInfo(MapMarkerInfo mapMarkerInfo)
        {
            // try to remove the marker.
            var deadMarker = mapMarkerInfos.FirstOrDefault(x => x.Equals(mapMarkerInfo));
            if (deadMarker != null)
            {
                return mapMarkerInfos.Remove(deadMarker);
            }

            return false;
        }
    }
}
