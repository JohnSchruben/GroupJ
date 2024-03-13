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


        protected void AddMapMarkerInfo(MapMarkerInfo mapMarkerInfo)
        {
            // don't add if it's null
            if (mapMarkerInfo == null)
            {
                return;
            }

            // don't add if it's a duplicate
            if (!mapMarkerInfos.Any(x => x.Equals(mapMarkerInfo)))
            {
                mapMarkerInfos.Add(mapMarkerInfo);
            }
        }

        protected void RemoveMapMarkerInfo(MapMarkerInfo mapMarkerInfo)
        {
            // don't remove if it's null
            if (mapMarkerInfo == null)
            {
                return;
            }

            // try to remove the marker.
            var deadMarker = mapMarkerInfos.FirstOrDefault(x => x.Equals(mapMarkerInfo));
            if (deadMarker != null)
            {
                mapMarkerInfos.Remove(deadMarker);
            }
        }
    }
}
