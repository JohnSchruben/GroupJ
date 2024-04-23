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
        private int count = 0;
        private ObservableCollection<MapMarkerInfo> markers = new ObservableCollection<MapMarkerInfo>();
        private MainPageViewModel mainPageViewModel;

        public MainPage()
        {
            InitializeComponent();
            //this.BindingContextChanged += MainPage_BindingContextChanged;
            //markers = new ObservableCollection<MapMarkerInfo>(ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.MapMarkerInfos);
            //this.map.ItemsSource = this.markers;
            //ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.MapMarkerInfos.CollectionChanged += this.MapMarkerInfos_CollectionChanged;
            //CheckAndRequestLocationPermission();
            //GetCurrentLocation();

            this.mainPageViewModel = this.BindingContext as MainPageViewModel;
            this.addMarkerView.BindingContext = this.mainPageViewModel.AddMarkerViewModel;

            Location centerLocation = new Location(35.207554, -97.444606);

            MapSpan mapSpan = MapSpan.FromCenterAndRadius(centerLocation, Distance.FromKilometers(0.5));

            map.MoveToRegion(mapSpan);
        }

        [Obsolete]
        private void MapMarkerInfos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (e.NewItems != null)
                {
                    foreach (MapMarkerInfo markerInfo in e.NewItems)
                    {
                        this.markers.Add(markerInfo);
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (MapMarkerInfo markerInfo in e.OldItems)
                    {
                        this.markers.Remove(markerInfo);
                    }
                }
            });
        }

        private void MapPinClicked(MapPin pin)
        {
            // Handle pin click
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

        private Location currentLocation;

        public async Task<Location> GetCurrentLocation()
        {
            try
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);
                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");

                    if (this.currentLocation == null || this.currentLocation != location)
                    {
                        this.currentLocation = location;
                        var newSpan = MapSpan.FromCenterAndRadius(this.currentLocation, Distance.FromKilometers(3)); 

                        this.map.MoveToRegion(newSpan);
                    }

                    return GetCurrentLocation().Result;
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

        private void map_MapClicked(object sender, MapClickedEventArgs e)
        {
            this.mainPageViewModel.AddMarkerViewModel.LoadMarker(new Coordinate(e.Location.Latitude, e.Location.Longitude, 10));
        }

        private void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
        {
            MapMarkerInfo mapMarkerInfo = ((Pin)sender).BindingContext as MapMarkerInfo;
            ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.RemoveMapMarkerInfo(mapMarkerInfo);
        }
    }
}