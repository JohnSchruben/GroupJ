using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate.Desktop
{
    public class MainWindowViewModel
    {
        private MapMarkerInfoCollectionProxy model;
        public MainWindowViewModel(MapMarkerInfoCollectionProxy model)
        {
           
            this.model = model; 
        }

        public IEnumerable<MapMarkerInfo> MarkerCollection => this.model.MapMarkerInfos;
    }
}
