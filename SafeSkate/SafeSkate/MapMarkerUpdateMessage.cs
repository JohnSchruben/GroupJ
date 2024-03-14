using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate
{
    [Serializable]
    public class MapMarkerUpdateMessage
    {
        public bool IsAdded { get; set; }
        public MapMarkerInfo Info { get; set; } 
    }
}
