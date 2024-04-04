using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows.Input;

namespace SafeSkate.Desktop
{
    public class MainWindowViewModel
    {
        private MapMarkerInfoCollectionProxy model;

        public MainWindowViewModel(MapMarkerInfoCollectionProxy model)
        {
            this.model = model;
        }

        public IEnumerable<MapMarkerInfo> MarkerCollection => this.model.MapMarkerInfos;

        public ICommand AddMarkerCommand => new RelayCommand(this.AddMarker);

        public async Task<Coordinate> GetLocationAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("http://ip-api.com/json/");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var locationData = JsonConvert.DeserializeObject<dynamic>(json);
                    return new Coordinate()
                    {
                        Latitude = locationData.lat,
                        Longitude = locationData.lon,
                    };
                }
            }

            return null;
        }

        private async void AddMarker()
        {
            // add new marker to model.
            var newInfo = await this.GetLocationAsync();
            this.model.AddMapMarkerInfo(new MapMarkerInfo(newInfo, "desktop client", DateTime.Now, Severity.ThePersonDied));
        }
    }
}