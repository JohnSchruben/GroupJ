using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate.Mobile
{
    public class AddMarkerViewModel : INotifyPropertyChanged
    {
        private MapMarkerInfoCollectionProxy model;
        private MapMarkerInfo newMarker;
        private bool visibility;

        public AddMarkerViewModel(MapMarkerInfoCollectionProxy model)
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


        public void GenerateMarker(Coordinate coordinate)
        {
            this.Visibility = true;
            this.newMarker = new MapMarkerInfo()
            {
                Location = coordinate,
            };
        }

        public void SubmitMarker()
        {
            this.newMarker.Severity = (Severity)this.Severity;
            this.newMarker.Description = this.Description;
            this.newMarker.TimeUploaded = DateTime.Now;
            this.model.AddMapMarkerInfo(this.newMarker);
            this.Visibility = false;
        }

        public void CloseUI()
        {
            this.Visibility = false;
        }
    }
}