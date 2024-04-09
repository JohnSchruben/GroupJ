using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace SafeSkate.Mobile
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            //Location oklahomaCity = new Location(35.4676, -97.5164);
            //MapSpan mapSpan = new MapSpan(oklahomaCity, 0.01, 0.01);
            //map = new Map(mapSpan);
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);

            ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.AddMapMarkerInfo(new MapMarkerInfo(new Coordinate(40.7128, -74.0060, 10), "mobile client", DateTime.Now, Severity.Morphine));
        }
    }

}
