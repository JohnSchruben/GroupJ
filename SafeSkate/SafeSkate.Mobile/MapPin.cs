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
        public MapMarkerInfo Model { get; set; }
        public string Id { get; set; }
        public Location Position { get; set; }
        public string Icon { get; set; }
        public ICommand EditCommand { get; set; }

        public MapPin(Action<MapPin> clicked, MapMarkerInfo model)
        {
            EditCommand = new Command(() => clicked(this));
            this.Model = model;
            Id = Guid.NewGuid().ToString();
            Position = new Location(model.Location.Latitude, model.Location.Longitude);
            Icon = model.Severity == Severity.FlintstonesVitamin ? "teal_pin" :
                model.Severity == Severity.BabyAspirin ? "yellow_pin" :
                model.Severity == Severity.Morphine ? "red_pin" : "magenta_pin";
        }
    }
}