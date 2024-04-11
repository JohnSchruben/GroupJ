using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace SafeSkate.Mobile
{
    /// <summary>
    ///   Ticket #( The Map
    /// </summary>
    public partial class MainPage : ContentPage
    {
        int count = 0;
        ObservableCollection<MapMarkerInfo> markers = new ObservableCollection<MapMarkerInfo>();
        public event Action MarkerAddRequested;
        Random random = new Random();

        // Define the range for latitude and longitude in the United States
        double minLatitude = 24.396308;
        double maxLatitude = 49.384358;
        double minLongitude = -125.001650;
        double maxLongitude = -66.934570;
  
        public MainPage()
        {

            InitializeComponent();
            markers = new ObservableCollection<MapMarkerInfo>(ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.MapMarkerInfos);
            this.map.ItemsSource = this.markers;
            ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.MapMarkerInfos.CollectionChanged += MapMarkerInfos_CollectionChanged;
            //Methods and event handlers for User Interaction with Map
            map.MapClicked += Map_MapClicked;
           
        }

        private void MapMarkerInfos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach(MapMarkerInfo markerInfo in e.NewItems)
                {
                    Device.BeginInvokeOnMainThread(() =>
                             {
                                 this.markers.Add(markerInfo);
                             });
                }
            }
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);

            // Generate a random latitude and longitude within the range
            double randomLatitude = random.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
            double randomLongitude = random.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

            ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.AddMapMarkerInfo(
                new MapMarkerInfo(new Coordinate(randomLatitude, randomLongitude, 10), "mobile client", DateTime.Now, Severity.Morphine));
        }

        [Obsolete]
        private async void Map_MapClicked(object? sender, Microsoft.Maui.Controls.Maps.MapClickedEventArgs e)
        {
            if (e != null)
            {
                Device.BeginInvokeOnMainThread(async () => {
                    //Prompt the user to add a description
                    string temp = await DisplayPromptAsync("Information", "Enter an approprite description of the added location", "OK", "Cancel", null, 256, null, "");
                    ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.AddMapMarkerInfo(new MapMarkerInfo(new Coordinate(e.Location.Latitude, e.Location.Longitude, 10), temp, DateTime.Now, Severity.ThePersonDied));
                    //e contains the clicked coordinates,so we grab them and store them in a local variable
                    SafeSkate.Coordinate Aelita = new Coordinate(e.Location.Latitude, e.Location.Longitude, 10);
                    MapMarkerInfo mapMarker = new MapMarkerInfo(Aelita, temp, DateTime.Now, Severity.ThePersonDied);
                    this.markers.Add(mapMarker);
                });
               
            }
                
           }
      

            

        }
      
    }

    


