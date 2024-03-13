using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate
{
    public class MapMarkerInfoCollectionProxy 
    {
        private MapMarkerInfoCollection model;
        private ServiceClient serviceClient;
        public MapMarkerInfoCollectionProxy(MapMarkerInfoCollection model, ServiceClient serviceClient)
        {
            this.model = model; 
            this.serviceClient = serviceClient;
            this.serviceClient.MapMarkerUpdateReceived += ServiceClient_MapMarkerUpdateReceived;
        }
        public ObservableCollection<MapMarkerInfo> MapMarkerInfos => this.model.MapMarkerInfos;  

        public void AddMapMarkerInfo(MapMarkerInfo mapMarkerInfo)
        {

            if (this.model.AddMapMarkerInfo(mapMarkerInfo))
            {
                var message = new MapMarkerUpdateMessage
                {
                    IsAdded = true,
                    Info = mapMarkerInfo,
                };

                this.serviceClient.PublishMapMarkerUpdate(message);
            }
        }
        public void RemoveMapMarkerInfo(MapMarkerInfo mapMarkerInfo)
        {
            if (this.model.RemoveMapMarkerInfo(mapMarkerInfo))
            {
                var message = new MapMarkerUpdateMessage
                {
                    Info = mapMarkerInfo,
                };

                this.serviceClient.PublishMapMarkerUpdate(message);
            }
        }

        private void ServiceClient_MapMarkerUpdateReceived(MapMarkerUpdateMessage message)
        {
            if (message.IsAdded)
            {
                this.model.AddMapMarkerInfo(message.Info);   
            }
            else
            {
                this.model.RemoveMapMarkerInfo(message.Info);
            }
        }
    }
}
