using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeSkate
{
    public class ServiceTypeProvider
    {
        private static ServiceTypeProvider instance;
        public static ServiceTypeProvider Instance => instance ?? (instance = new ServiceTypeProvider());
        public static IEnumerable<MapMarkerInfo> DefaultMapMarkerInfos { get; set; }

        public ServiceTypeProvider()
        {
        }

        private readonly Lazy<MapMarkerInfoCollectionProxy> mapMarkerInfoProxy =
            new Lazy<MapMarkerInfoCollectionProxy>(() => CreateProxy().Result);

        public static string ServerIp { get; set; }

        public static int UpdatePort { get; set; }
        public static int QueryPort { get; set; }

        public MapMarkerInfoCollectionProxy MapMarkerInfoCollectionProxy => mapMarkerInfoProxy.Value;

        private static async Task<MapMarkerInfoCollectionProxy> CreateProxy()
        {
            if (string.IsNullOrEmpty(ServerIp))
            {
                throw new InvalidOperationException("Server IP or Port is not configured properly.");
            }

            var serviceClient = new ServiceClient(ServerIp, UpdatePort, QueryPort);
            var mapMarkers = new List<MapMarkerInfo>();

            await serviceClient.StartAsync();

            if (DefaultMapMarkerInfos == null)
            {
                mapMarkers = await serviceClient.QueryMarkersAsync("get");
            }
            else
            {
                mapMarkers = (List<MapMarkerInfo>)DefaultMapMarkerInfos;
            }

            return new MapMarkerInfoCollectionProxy(new MapMarkerInfoCollection(mapMarkers), serviceClient);
        }
    }
}