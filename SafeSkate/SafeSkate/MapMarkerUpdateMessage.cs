namespace SafeSkate
{
    [Serializable]
    public class MapMarkerUpdateMessage
    {
        public bool IsAdded { get; set; }
        public MapMarkerInfo Info { get; set; }
    }
}