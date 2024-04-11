using Microsoft.Maui.Controls.Maps;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SafeSkate.Mobile
{
    internal class MainPageViewModel
    {
        private MapMarkerInfoCollectionProxy model;

        public MainPageViewModel(MapMarkerInfoCollectionProxy model)
        {
            this.model = model;
        }

        public IEnumerable<MapMarkerInfo> MarkerCollection => this.model.MapMarkerInfos;


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
