using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate.Mobile
{
    public class EditMarkerViewModel : INotifyPropertyChanged
    {
        private MapMarkerInfoCollectionProxy model;
        private MapMarkerInfo currentMarker;
        private bool visibility;

        public EditMarkerViewModel(MapMarkerInfoCollectionProxy model)
        {
            this.model = model;
            this.Description = "Hazard";
        }

        public bool Visibility
        {
            get { return this.visibility; }
            set
            {
                if (this.visibility != value)
                {
                    this.visibility = value;
                    this.OnPropertyChanged(nameof(this.Description));
                    this.OnPropertyChanged(nameof(this.Severity));
                    this.OnPropertyChanged(nameof(this.Visibility));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public int Severity { get; set; }
        public string Description { get; set; }

        public void LoadMarker(MapMarkerInfo markerInfo)
        {
            this.currentMarker = markerInfo;
            this.Severity = (int)markerInfo.Severity;
            this.OnPropertyChanged(nameof(this.Severity));
            this.Description = markerInfo.Description;
            this.Visibility = true;
        }

        public void SaveMarker()
        {
            var newMarker = new MapMarkerInfo(this.currentMarker.Location, this.Description, DateTime.Now, (Severity)this.Severity);
            this.model.AddMapMarkerInfo(newMarker);
            Thread.Sleep(1000);
            this.model.RemoveMapMarkerInfo(currentMarker);
            this.Visibility = false;
        }

        public void DeleteMarker()
        {
            this.model.RemoveMapMarkerInfo(currentMarker);
            this.Visibility = false;
        }

        public void CloseUI()
        {
            this.Visibility = false;
        }
    }
}