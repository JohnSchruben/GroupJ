using Microsoft.Maui.Controls.Maps;
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

        double minLatitude = 35.203977;
        double maxLatitude = 35.211130;
        double minLongitude = -97.441307;
        double maxLongitude = -97.447905;
     
        public MainPage()
        {
            InitializeComponent();
            markers = new ObservableCollection<MapMarkerInfo>(ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.MapMarkerInfos);
            this.map.ItemsSource = this.markers;
            ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.MapMarkerInfos.CollectionChanged += MapMarkerInfos_CollectionChanged;
            CheckAndRequestLocationPermission();
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

    public async Task CheckAndRequestLocationPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        // Optionally check for background location as needed:
        // var backgroundStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();
    }
    public async Task<Location> GetCurrentLocation()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(request);
            if (location != null)
            {
                Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");

                    ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.AddMapMarkerInfo(
                        new MapMarkerInfo(new Coordinate(location.Latitude, location.Longitude, 10), "john's mobile client", DateTime.Now, Severity.Morphine));
                    return location;
            }
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            // Handle not supported on device exception
        }
        catch (PermissionException pEx)
        {
            // Handle permission exception
        }
        catch (Exception ex)
        {
            // Unable to get location
        }

        return null;
    }

    private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            GetCurrentLocation();
            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void map_MapClicked(object sender, Microsoft.Maui.Controls.Maps.MapClickedEventArgs e)
        {
            ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.AddMapMarkerInfo(
                new MapMarkerInfo(new Coordinate(e.Location.Latitude, e.Location.Longitude, 10), "john's mobile client", DateTime.Now, Severity.Morphine));
        }

        private void Pin_MarkerClicked(object sender, Microsoft.Maui.Controls.Maps.PinClickedEventArgs e)
        {
            MapMarkerInfo mapMarkerInfo = ((Pin)sender).BindingContext as MapMarkerInfo;
            ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.RemoveMapMarkerInfo(mapMarkerInfo);
        }
    }

}
