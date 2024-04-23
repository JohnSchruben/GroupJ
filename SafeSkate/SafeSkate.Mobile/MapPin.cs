using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SafeSkate.Mobile
{
    public class MapPin
    {
        public string Id { get; set; }
        public Location Position { get; set; }
        public string Icon { get; set; }
        public ICommand ClickedCommand { get; set; }

        public MapMarkerInfo Model { get; set; }

        public MapPin(Action<MapPin> clicked, MapMarkerInfo model)
        {
            ClickedCommand = new Command(() => clicked(this));
            this.Model = model;
            Id = Guid.NewGuid().ToString();
            Position = new Location(model.Location.Latitude, model.Location.Longitude);
            Icon = model.Severity == Severity.FlintstonesVitamin ? "teal_pin" :
                model.Severity == Severity.BabyAspirin ? "yellow_pin" :
                model.Severity == Severity.Morphine ? "red_pin" : "magenta_pin";
        }
    }
}