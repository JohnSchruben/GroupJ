using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SafeSkate.Mobile
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        private MapMarkerInfoCollectionProxy model;
        private AddMarkerViewModel addMarkerViewModel;
        public MainPageViewModel(MapMarkerInfoCollectionProxy model, AddMarkerViewModel addMarkerViewModel)
        {
            this.model = model;
            this.addMarkerViewModel = addMarkerViewModel;
            Pins = new ObservableCollection<MapPin>();

            foreach(var marker in this.model.MapMarkerInfos)
            {
                this.Pins.Add(new MapPin(MapPinClicked, marker)); 
            }

            this.model.MapMarkerInfos.CollectionChanged += MapMarkerInfos_CollectionChanged;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MapMarkerInfos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (e.NewItems != null)
                {
                    foreach (MapMarkerInfo markerInfo in e.NewItems)
                    {
                        this.Pins.Add(new MapPin(MapPinClicked, markerInfo));
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (MapMarkerInfo markerInfo in e.OldItems)
                    {
                        var oldPin = this.Pins.FirstOrDefault(x => x.Model.Equals(markerInfo));
                        this.Pins.Remove(oldPin);
                    }
                }
            });

        }

        private void MapPinClicked(MapPin pin)
        {
            this.model.RemoveMapMarkerInfo(pin.Model);
        }

        private ObservableCollection<MapPin> pins;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<MapPin> Pins
        {
            get { return pins; }
            set { pins = value;  }
        }
        public IEnumerable<MapMarkerInfo> MarkerCollection => this.model.MapMarkerInfos;
        public AddMarkerViewModel AddMarkerViewModel => this.addMarkerViewModel;
    }
}
