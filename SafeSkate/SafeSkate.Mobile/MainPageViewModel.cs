using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SafeSkate.Mobile
{
    internal class MainPageViewModel
    {
        private MapMarkerInfoCollectionProxy model;
        private AddMarkerViewModel addMarkerViewModel;
        public MainPageViewModel(MapMarkerInfoCollectionProxy model, AddMarkerViewModel addMarkerViewModel)
        {
            this.model = model;
            this.addMarkerViewModel = addMarkerViewModel;
        }

        public IEnumerable<MapMarkerInfo> MarkerCollection => this.model.MapMarkerInfos;
        public AddMarkerViewModel AddMarkerViewModel => this.addMarkerViewModel;
    }
}
