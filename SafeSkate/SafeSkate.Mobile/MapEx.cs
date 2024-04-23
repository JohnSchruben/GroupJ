using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate.Mobile
{
    public class MapEx : Microsoft.Maui.Controls.Maps.Map
    {
        public ObservableCollection<MapPin> CustomPins
        {
            get { return (ObservableCollection<MapPin>)GetValue(CustomPinsProperty); }
            set { SetValue(CustomPinsProperty, value); }
        }

        public static readonly BindableProperty CustomPinsProperty = BindableProperty.Create(nameof(CustomPins), typeof(ObservableCollection<MapPin>), typeof(MapEx), null);
    }
}
