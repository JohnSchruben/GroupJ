using System;
using System.Runtime.Serialization;

namespace SafeSkate
{
    [Serializable]
    public class MapMarkerUpdateMessage
    {
        [DataMember]
        public bool IsAdded { get; set; }

        [DataMember]
        public MapMarkerInfo Info { get; set; }

        [DataMember]
        public Guid ProcessId { get; set; } 
    }
}