using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Unity.Injection;
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

        //For Geolocation (Used in GetCurrentLocation)
        private Location currentLocation;
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
        private MapMarkerInfo userLoc;


        public MainPage()
        {
            InitializeComponent();

            this.mainPageViewModel = this.BindingContext as MainPageViewModel;
            this.addMarkerView.BindingContext = this.mainPageViewModel.AddMarkerViewModel;

            Location centerLocation = new Location(35.207554, -97.444606);

            MapSpan mapSpan = MapSpan.FromCenterAndRadius(centerLocation, Distance.FromKilometers(0.5));

            map.MoveToRegion(mapSpan);

            addButton.IsVisible = true;
            cancelButton.IsVisible = false;

            RegisterLocationListener();
            Add_Current_Location();
        }

        private void PlayAlertSound()
        {
            try
            {
                
                #if ANDROID
                var audioPlayer = new SafeSkate.Mobile.Platforms.Android.AudioPlayer();
                audioPlayer.PlaySound();
                #elif IOS
                DependencyService.Get<IAudioPlayer>().PlaySound();
                #endif
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }


            

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
                        this.PlayAlertSound();
                    }
                }
            });
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
        public async void RegisterLocationListener()
        {
            try
            {
                Geolocation.LocationChanged += Geolocation_LocationChanged;
                var request = new GeolocationListeningRequest(GeolocationAccuracy.Medium);
                await Geolocation.StartListeningForegroundAsync(request);
            }
            catch (Exception e)
            {
                // Unable to listen for location changes
            }
        }


        public async Task<Location?> GetCurrentLocation()
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                    currentLocation = location;
                else
                    currentLocation = new Location(location);

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                CancelRequest();
                await DisplayAlert("Unsupported HW Detected:", fnsEx.ToString(), "Exit");
            }
            catch (PermissionException pEx)
            {
                CancelRequest();
                await DisplayAlert("Insufficent Permissions:", pEx.ToString(), "Exit");
            }
            catch (Exception ex)
            {
                CancelRequest();
                await DisplayAlert("Unable to Get Location or Other:", ex.ToString(), "Exit");
            }

            return null;
        }

       public async void OnStartListening()
        {
            try
            {
                Geolocation.LocationChanged += Geolocation_LocationChanged;
                var request = new GeolocationListeningRequest(GeolocationAccuracy.Default);
                var success = await Geolocation.StartListeningForegroundAsync(request);

                string status = success
                    ? "Started listening for foreground location updates"
                    : "Couldn't start listening";
                Thread.Sleep(TimeSpan.FromSeconds(1));
                Location update = await Geolocation.GetLocationAsync();
               
                //Notify User that Service is started and running
                await DisplayAlert("Status", status, "OK");
            }
            catch (Exception ex)
            {
                 await DisplayAlert("ERROR:", ex.ToString(), "Exit");
            }
        }

       public async void Geolocation_LocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
        {
            if (currentLocation != null)
            {
                this.currentLocation = e.Location;
                this.userLoc.Location.Latitude = e.Location.Latitude;
                this.userLoc.Location.Longitude = e.Location.Longitude;
                MapMarkerInfo temp = new MapMarkerInfo(new Coordinate(e.Location.Latitude,e.Location.Longitude,10),"User",DateTime.Now,Severity.Morphine);
                temp.Description = "User Location";

                ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.AddMapMarkerInfo(temp);
                this.userLoc = temp;
                Thread.Sleep(TimeSpan.FromSeconds(1));
                this.PlayAlertSound();
                await DisplayAlert("New User Location:", currentLocation.Latitude.ToString() + "," + currentLocation.Longitude.ToString(), "OK");

            }
            else
                currentLocation = new Location(e.Location);
        }

      public void OnStopListening()
      {
            try
            {
                Geolocation.LocationChanged -= Geolocation_LocationChanged;
                Geolocation.StopListeningForeground();
                string status = "Stopped listening for foreground location updates";
                Task.Run(async () => await DisplayAlert("Status", status, "OK"));
            }
            catch (Exception ex)
            {
                Task.Run(async () => await DisplayAlert("ERROR:", ex.ToString(), "Exit"));
            }
        }

        private void map_MapClicked(object sender, MapClickedEventArgs e)
        {
            if (!this.addButton.IsVisible)
            {
                this.mainPageViewModel.AddMarkerViewModel.GenerateMarker(new Coordinate(e.Location.Latitude, e.Location.Longitude, 10));
                this.addButton.IsVisible = true;
                this.cancelButton.IsVisible = false;
            }
        }

        private void Add_Clicked(object sender, EventArgs e)
        {
            addButton.IsVisible = false;
            cancelButton.IsVisible = true;
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            addButton.IsVisible = true;
            cancelButton.IsVisible = false;
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }

        
        private async void Add_Current_Location()
        {
            await CheckAndRequestLocationPermission();
            await GetCurrentLocation();
            if (currentLocation != null)
            {
                //Add marker
                 this.userLoc = new MapMarkerInfo(new Coordinate(currentLocation.Latitude,currentLocation.Longitude,10),"User",DateTime.Now,Severity.BabyAspirin);
                 this.userLoc.Description = "Your Current Location";
                ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.AddMapMarkerInfo(userLoc);   
               
             
                //Begin updating location
                await DisplayAlert("User Coordinates", currentLocation.Latitude.ToString() + "," + currentLocation.Longitude.ToString(), "OK");
                //PlayAlertSound();
                OnStartListening();
            }
            else
            {
                await DisplayAlert("ERROR", "Failed to Add Current Location! Ensure you allowed Safeskate to use Location service!", "Exit");
                //We failed, notify user and clean up
            }
                

            
        }
    }
}