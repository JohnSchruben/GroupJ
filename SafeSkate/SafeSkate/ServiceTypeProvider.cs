using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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

        // Lazy initialization for MapMarkerInfoProxy using the factory method
        private readonly Lazy<MapMarkerInfoCollectionProxy> mapMarkerInfoProxy =
            new Lazy<MapMarkerInfoCollectionProxy>(() => CreateProxy().Result);

        // Static properties for server configuration
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
