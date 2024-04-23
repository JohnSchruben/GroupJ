using System.Collections.ObjectModel;

namespace SafeSkate
{
    public class MapMarkerInfoCollectionProxy
    {
        private MapMarkerInfoCollection model;
        private ServiceClient serviceClient;
        private Guid processId;

        public MapMarkerInfoCollectionProxy(MapMarkerInfoCollection model, ServiceClient serviceClient)
        {
            this.model = model;
            this.serviceClient = serviceClient;
            this.serviceClient.MapMarkerUpdateReceived += this.ServiceClient_MapMarkerUpdateReceived;
            this.processId = Guid.NewGuid();
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
                    ProcessId = processId   
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
                    IsAdded = false,
                    Info = mapMarkerInfo,
                    ProcessId = processId
                };

                // make and publish the update.
                this.serviceClient.PublishMapMarkerUpdate(message);
            }
        }

        private void ServiceClient_MapMarkerUpdateReceived(MapMarkerUpdateMessage message)
        {
            if (this.processId == message.ProcessId)
            {
                return;
            }

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