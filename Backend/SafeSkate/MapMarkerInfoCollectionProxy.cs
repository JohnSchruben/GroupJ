using System.Collections.ObjectModel;

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
            this.serviceClient.MapMarkerUpdateReceived += this.ServiceClient_MapMarkerUpdateReceived;
        }

        public ObservableCollection<MapMarkerInfo> MapMarkerInfos => this.model.MapMarkerInfos;

        public void AddMapMarkerInfo(MapMarkerInfo mapMarkerInfo)
        {
            // check if the model updated.
            if (this.model.AddMapMarkerInfo(mapMarkerInfo))
            {
                var message = new MapMarkerUpdateMessage
                {
                    IsAdded = true,
                    Info = mapMarkerInfo,
                };

                // make and publish the update.
                this.serviceClient.PublishMapMarkerUpdate(message);
            }
        }

        public void RemoveMapMarkerInfo(MapMarkerInfo mapMarkerInfo)
        {
            // check if the model updated.
            if (this.model.RemoveMapMarkerInfo(mapMarkerInfo))
            {
                var message = new MapMarkerUpdateMessage
                {
                    Info = mapMarkerInfo,
                };

                // make and publish the update.
                this.serviceClient.PublishMapMarkerUpdate(message);
            }
        }

        private void ServiceClient_MapMarkerUpdateReceived(MapMarkerUpdateMessage message)
        {
            // we received an update from someone else. Handle it to get up to date.
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
