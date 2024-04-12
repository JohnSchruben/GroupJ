using System.Collections.ObjectModel;
using System.Windows;

namespace SafeSkate.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<MapMarkerInfo> mapMarkers;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAsync();
            this.Loaded += async (sender, e) => await Window_LoadedAsync(sender, e);
        }

        private async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.NavigateToString(@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Simple Map</title>
                    <script>
                    let map;

                    function initMap() {
                        var location = { lat: 35.211166, lng: -97.441197 };
                        map = new google.maps.Map(document.getElementById('map'), {
                            zoom: 8,
                            center: location
                        });
                    }

                    function addMarker(lat, lng) {
                        new google.maps.Marker({
                            position: { lat: lat, lng: lng },
                            map: map
                        });
                    }

                    </script>
                    <script src=""https://maps.googleapis.com/maps/api/js?key=AIzaSyBvTpNnvy_5fNhvFrRrtZ8_NYVlG4P-xcY&callback=initMap"" async defer></script>
                </head>
                <body>
                    <div id=""map"" style=""width: 100%; height: 100vh;""></div>
                </body>
                </html>
                ");
        }

        private async Task Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            DataContext = ViewModelLocator.Instance.MainWindowViewModel;
            this.mapMarkers = (this.DataContext as MainWindowViewModel).MarkerCollection as ObservableCollection<MapMarkerInfo>;

            var list = new List<MapMarkerInfo>(this.mapMarkers);
            foreach (MapMarkerInfo marker in list)
            {
                this.AddMarker(marker.Location.Latitude, marker.Location.Longitude);
            }
            this.mapMarkers.CollectionChanged += MapMarkers_CollectionChanged;
        }

        private void MapMarkers_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MapMarkerInfo marker in e.NewItems)
                {
                    this.AddMarker(marker.Location.Latitude, marker.Location.Longitude);
                }
            }
        }

        public async void AddMarker(double lat, double lng)
        {
            Application.Current.Dispatcher.Invoke(async () => 
            {
                if (webView?.CoreWebView2 != null)
                {
                    await webView.CoreWebView2.ExecuteScriptAsync($"addMarker({lat}, {lng})");
                }
            });
        }

        private async void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (this.mapMarkers != null)
            {
                var list = new List<MapMarkerInfo>(this.mapMarkers);
                foreach (var marker in list)
                {
                    this.AddMarker(marker.Location.Latitude, marker.Location.Longitude);
                }
            }
        }
    }
}