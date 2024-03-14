using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public ICommand AddMarkerCommand => new RelayCommand(this.AddMarker);

        private void AddMarker()
        {
            // add new marker to model.
        }
    }
}
